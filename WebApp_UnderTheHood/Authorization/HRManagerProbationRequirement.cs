using Microsoft.AspNetCore.Authorization;

namespace WebApp_UnderTheHood.Authorization
{
	public class HRManagerProbationRequirement : IAuthorizationRequirement
	{
        public HRManagerProbationRequirement(int probationMonths)
        {
			ProbationMonths = probationMonths;
		}

		public int ProbationMonths { get; }
	}

	//Fazer a mesma coisa na webAPI se quiser usar os Requirements
	public class HRManagerProbationRequirementHandler : AuthorizationHandler<HRManagerProbationRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRManagerProbationRequirement requirement)
		{
			if(!context.User.HasClaim(x => x.Type == "EmploymentDate"))
				return Task.CompletedTask;

			var empDate = DateTime.Parse(context.User.FindFirst(x => x.Type == "EmploymentDate").Value);
			var period = DateTime.Now - empDate;
			if (period.Days > 30 * requirement.ProbationMonths)
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}
