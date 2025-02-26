﻿using CleanArchitecture_2025.Domain.Employees;
using FluentValidation;
using GenericRepository;
using Mapster;
using MediatR;
using TS.Result;

namespace CleanArchitecture_2025.Application.Employees
{
    public sealed record EmployeeCreateCommand
    (
        string FirstName,
        string LastName,
        DateOnly BirthOfDate,
        decimal Salary,
        PersonalInformation PersonalInformation,
        Address? Address
    ) : IRequest<Result<string>>;

    public sealed class EmployeeCreateCommandValidator : AbstractValidator<EmployeeCreateCommand>
    {
        public EmployeeCreateCommandValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MinimumLength(3).WithMessage("FirstName must be at least 3 characters");
            RuleFor(x => x.LastName).NotEmpty().MinimumLength(2).WithMessage("LastName must be at least 2 characters");
            RuleFor(x => x.BirthOfDate).NotEmpty();
            RuleFor(x => x.Salary).NotEmpty();
            RuleFor(x => x.PersonalInformation.TCNo).NotEmpty()
                .MaximumLength(11).WithMessage("Please check your TC No")
                .MinimumLength(11).WithMessage("Please check your TC No");
        }
    }
    internal sealed class EmployeeCreateCommandHandler(IEmployeeRepository employeeRepository,IUnitOfWork unitOfWork) : IRequestHandler<EmployeeCreateCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(EmployeeCreateCommand request, CancellationToken cancellationToken)
        {
            var isEmployeeExist = await employeeRepository.AnyAsync(x => x.PersonalInformation.TCNo == request.PersonalInformation.TCNo, cancellationToken);

            if (isEmployeeExist)
            {
                return Result<string>.Failure("Employee already exists");
            }

            Employee employee = request.Adapt<Employee>();
            employeeRepository.Add(employee);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return "Employee registered successfully";
        }
    }
}
