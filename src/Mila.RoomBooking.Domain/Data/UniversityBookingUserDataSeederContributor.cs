using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using UniversityBooking.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using IdentityRole = Volo.Abp.Identity.IdentityRole;

namespace UniversityBooking.Data
{
    public class UniversityBookingUserDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IIdentityRoleRepository _roleRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IPermissionManager _permissionManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IConfiguration _configuration;
        private readonly IdentityUserManager _userManager;
        private readonly IdentityRoleManager _roleManager;

        public UniversityBookingUserDataSeederContributor(
            IIdentityRoleRepository roleRepository,
            IIdentityUserRepository userRepository,
            IPermissionManager permissionManager,
            IGuidGenerator guidGenerator,
            IConfiguration configuration,
            IdentityUserManager userManager,
            IdentityRoleManager roleManager)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _permissionManager = permissionManager;
            _guidGenerator = guidGenerator;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Create roles
            await CreateRolesAsync();

            // Create users
            await CreateUsersAsync();

            // Assign permissions to roles
            await AssignPermissionsToRolesAsync();
        }

        private async Task CreateRolesAsync()
        {
            // Define roles we want to create
            var rolesToCreate = new List<(string Name, string DisplayName)>
            {
                ("admin", "Administrator"),
                ("instructor", "Instructor"),
                ("student", "Student"),
                ("room_manager", "Room Manager")
            };

            foreach (var roleInfo in rolesToCreate)
            {
                // Check if role already exists
                var roleExists = await _roleRepository.FindByNormalizedNameAsync(roleInfo.Name.ToUpper());
                if (roleExists == null)
                {
                    var role = new IdentityRole(
                        _guidGenerator.Create(),
                        roleInfo.Name)
                    {
                        IsStatic = false,
                        IsPublic = true,
                        IsDefault = roleInfo.Name == "student" // Make student the default role
                    };

                    await _roleRepository.InsertAsync(role);
                }
            }
        }

        private async Task CreateUsersAsync()
        {
            // Define users to create - in a real scenario, you might want to read this from a configuration
            var usersToCreate = new List<(string UserName, string Email, string Password, string Role, string FullName)>
            {
                ("admin", "admin@university.edu", "P@ssw0rd123!", "admin", "Admin User"),
                ("instructor1", "instructor1@university.edu", "Instr@123", "instructor", "John Smith"),
                ("instructor2", "instructor2@university.edu", "Instr@123", "instructor", "Emily Johnson"),
                ("student1", "student1@university.edu", "Student@123", "student", "Michael Wilson"),
                ("student2", "student2@university.edu", "Student@123", "student", "Sarah Brown"),
                ("roommanager", "roommanager@university.edu", "M@nager123", "room_manager", "Room Manager")
            };

            foreach (var userInfo in usersToCreate)
            {
                // Check if user already exists
                var userExists = await _userRepository.FindByNormalizedUserNameAsync(userInfo.UserName.ToUpper());
                if (userExists == null)
                {
                  var user = new IdentityUser(
                    _guidGenerator.Create(),
                    userInfo.UserName,
                    userInfo.Email)

                    {
                        Name = userInfo.FullName.Split(' ')[0],
                        Surname = userInfo.FullName.Split(' ').Length > 1 ? userInfo.FullName.Split(' ')[1] : ""
                    };

                    // Set password
                    (await _userManager.CreateAsync(user, userInfo.Password)).CheckErrors();

                    // Add user to role
                    (await _userManager.AddToRoleAsync(user, userInfo.Role)).CheckErrors();
                }
            }
        }

        private async Task AssignPermissionsToRolesAsync()
        {
            // Define permission assignments
            var permissionAssignments = new List<(string RoleName, string Permission)>
            {
                // Admin role - grant all permissions
                ("admin", UniversityBookingPermissions.BookingRequest.Default),
                ("admin", UniversityBookingPermissions.BookingRequest.Create),
                ("admin", UniversityBookingPermissions.BookingRequest.View),
                ("admin", UniversityBookingPermissions.BookingRequest.Manage),
                ("admin", UniversityBookingPermissions.Room.Default),
                ("admin", UniversityBookingPermissions.Room.Create),
                ("admin", UniversityBookingPermissions.Room.Update),
                ("admin", UniversityBookingPermissions.Room.Delete),
                ("admin", UniversityBookingPermissions.BookingRequest.Default),
                ("admin", UniversityBookingPermissions.BookingRequest.Default),
                ("admin", UniversityBookingPermissions.BookingRequest.Manage),


                // Instructor permissions
                ("instructor", UniversityBookingPermissions.BookingRequest.Default),
                ("instructor", UniversityBookingPermissions.BookingRequest.Create),
                ("instructor", UniversityBookingPermissions.BookingRequest.View),

                // Student permissions
                ("student", UniversityBookingPermissions.BookingRequest.Default),
                ("student", UniversityBookingPermissions.BookingRequest.View),

                // Room Manager permissions
                ("room_manager", UniversityBookingPermissions.BookingRequest.Default),
                ("room_manager", UniversityBookingPermissions.BookingRequest.View),
                ("room_manager", UniversityBookingPermissions.BookingRequest.Manage),
                ("room_manager", UniversityBookingPermissions.Room.Default),
                ("room_manager", UniversityBookingPermissions.Room.Create),
                ("room_manager", UniversityBookingPermissions.Room.Update)
            };

            // Group by role to reduce number of database calls
            var permissionsByRole = permissionAssignments
                .GroupBy(x => x.RoleName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Permission).ToList());

            foreach (var role in permissionsByRole.Keys)
            {
                foreach (var permission in permissionsByRole[role])
                {
                    await _permissionManager.SetForRoleAsync(role, permission, true);
                }
            }
        }
    }
}
