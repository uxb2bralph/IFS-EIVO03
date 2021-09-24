using Model.DataEntity;
using Model.Locale;
using Model.Models.ViewModel;
using Model.Properties;
using Model.Security.MembershipManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Model.Helper
{
    public static class AuthExtensions
    {
        public static String ComputeAuthorization(this OrganizationToken token, SHA256 hash, String seed)
        {
            String computedAuth = Convert.ToBase64String(
                    Encoding.Default.GetBytes(
                        String.Concat(
                            hash.ComputeHash(
                                Encoding.Default.GetBytes($"{token.Organization.ReceiptNo}{token.KeyID}{seed}")
                            ).Select(b => b.ToString("x2"))
                        )
                    )
                );

            return computedAuth;
        }

        public static bool IsSystemAdmin(this UserProfileMember profile)
        {
            return profile != null && profile.CurrentUserRole.RoleID == (int)Naming.RoleID.ROLE_SYS;
        }

        public static bool IsAuthorized(this UserProfileMember profile, Naming.RoleID[] roleID)
        {
            return profile != null && roleID.Contains((Naming.RoleID)profile.CurrentUserRole.RoleID);
        }
    }
}
