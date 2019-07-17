using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WpfApp1
{
    public class TetsUnit
    {
        string filePath = @"G:\1.txt";
        string folderPath = @"G:\xx";
    


        [Fact]
        public void TestFlie()
        {
            PermissionManager.AddSecurityControll2File(filePath);
        }

        [Fact]
        public void TestFolder()
        {
            //PermissionManager.xx(folderPath);
        }


        [Fact]
        public void FormatDisk()
        {
            DriveDiskManager.SetLabel('H',"哼");
        }
    }
}
