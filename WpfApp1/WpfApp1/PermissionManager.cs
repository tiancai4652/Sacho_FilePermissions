using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class PermissionManager
    {
        /// <summary>
        /// 为文件添加users，everyone用户组的完全控制权限
        /// </summary>
        /// <param name="filePath"></param>
        public static void AddSecurityControll2File(string filePath)
        {

            //获取文件信息
            FileInfo fileInfo = new FileInfo(filePath);
            //获得该文件的访问权限
            System.Security.AccessControl.FileSecurity fileSecurity = fileInfo.GetAccessControl();
            //添加ereryone用户组的访问权限规则 完全控制权限
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, AccessControlType.Allow));
            //添加Users用户组的访问权限规则 完全控制权限
            fileSecurity.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));
            //设置访问权限
            fileInfo.SetAccessControl(fileSecurity);
        }

        /// <summary>
        ///为文件夹添加users，everyone用户组的完全控制权限
        /// </summary>
        /// <param name="dirPath"></param>
        public static void AddSecurityControll2Folder(string dirPath)
        {
            //获取文件夹信息
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            //获得该文件夹的所有访问权限
            System.Security.AccessControl.DirectorySecurity dirSecurity = dir.GetAccessControl(AccessControlSections.All);
            //设定文件ACL继承
            InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            //添加ereryone用户组的访问权限规则 完全控制权限
            FileSystemAccessRule everyoneFileSystemAccessRule = new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
            //添加Users用户组的访问权限规则 完全控制权限
            FileSystemAccessRule usersFileSystemAccessRule = new FileSystemAccessRule("Users", FileSystemRights.FullControl, inherits, PropagationFlags.None, AccessControlType.Allow);
            bool isModified = false;
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule, out isModified);
            dirSecurity.ModifyAccessRule(AccessControlModification.Add, usersFileSystemAccessRule, out isModified);
            //设置访问权限
            dir.SetAccessControl(dirSecurity);
        }


        /// <summary>
        /// 为文件夹移除某个用户的权限
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="username"></param>
        static void removePermissions(string dirName, string username)
        {
            string user = System.Environment.UserDomainName + "\\" + username;
            DirectoryInfo dirinfo = new DirectoryInfo(dirName);
            DirectorySecurity dsec = dirinfo.GetAccessControl(AccessControlSections.All);

            AuthorizationRuleCollection rules = dsec.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            foreach (AccessRule rule in rules)
            {
                if (rule.IdentityReference.Value == user)
                {
                    bool value;
                    dsec.PurgeAccessRules(rule.IdentityReference);
                    dsec.ModifyAccessRule(AccessControlModification.RemoveAll, rule, out value);
                }
            }
        }

        /// <summary>
        /// 项目中用，文件夹只保留everyone权限，其中允许用户读，但不允许写
        /// by the way,代码结果是给文件夹一个特殊权限，点进去高级看，会发现这个特殊权限的子项和写入权限的子项是一样的
        /// </summary>
        /// <param name="dirName"></param>
        public static void OnlyKeepEveryonePermissionsWithWriteNotAllowed(string dirName)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(dirName);
            DirectorySecurity objSecObj = dirinfo.GetAccessControl();
            AuthorizationRuleCollection acl = objSecObj.GetAccessRules(true, true,
                                                        typeof(System.Security.Principal.NTAccount));
            objSecObj.SetAccessRuleProtection(true, false); //to remove inherited permissions
            foreach (FileSystemAccessRule ace in acl) //to remove any other permission
            {
                objSecObj.PurgeAccessRules(ace.IdentityReference);  //same as use objSecObj.RemoveAccessRuleSpecific(ace);
            }
            InheritanceFlags inherits = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
            FileSystemAccessRule everyoneFileSystemAccessRule = new FileSystemAccessRule("Everyone", FileSystemRights.ReadAndExecute | FileSystemRights.ListDirectory | FileSystemRights.Read, inherits, PropagationFlags.None, AccessControlType.Allow);
            FileSystemAccessRule everyoneFileSystemAccessRule2 = new FileSystemAccessRule("Everyone", FileSystemRights.Write, AccessControlType.Deny);
            bool isModified = false;
            objSecObj.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule2, out isModified);
            objSecObj.ModifyAccessRule(AccessControlModification.Add, everyoneFileSystemAccessRule, out isModified);
            dirinfo.SetAccessControl(objSecObj);
        }
    }
}
