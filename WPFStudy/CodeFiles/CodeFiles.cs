using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WPFStudy.CodeFile
{
    // 代码说明与代码文件路径
    public static class CodeFiles
    {
        // 代码说明标准长度
        private static readonly int CodeFileStandardLength = 10;

        // 代码说明
        private static string[] CodeFileExplanation = new string[]
        { 
            "Grid布局容器: ",
            "StackPanel布局容器: ",
            "WrapPanel布局容器: ",
            "DockPanel布局容器: ",
        };

        // 代码路径
        private static string[] CodeFilePath = new string[]
        {
            "../Layout/LayoutGrid.xaml",
            "../Layout/LayoutStackPanel.xaml",
            "../Layout/LayoutWrapPanel.xaml",
            "../Layout/LayoutDockPanel.xaml",
        };



        // 循环输出所有的代码说明和代码文件路径
        public static void CodeFileOutput()
        {
            // 定义一个字符串,用于存储代码说明和代码文件路径
            string Output = string.Empty;

            // 循环输出
            for(int  i = 0; i < CodeFileExplanation.Length; i++)
            {
                // 先输出代码说明
                Debug.Write(i > 9 ? $"{i + 1}.": $"{i + 1} .");
                Debug.Write(CodeFileExplanation[i]);
                // 在代码文件路径中间补充(点.)
                for(int j = 0; j < CodeFileStandardLength - CodeFileExplanation[i].Length; j++)
                {
                    Debug.Write("..");
                }
                // 最后输出代码文件路径
                Debug.WriteLine(CodeFilePath[i]);
            }
        }

        

        // 计算字符串显示的长度
        private static int GetStringWidth(string str)
        {
            // 如果字符串是空,直接返回0
            if(string.IsNullOrEmpty(str)) return 0;

            // 如果字符串不为空,则计算所有字符的长度
            // 定义一个Width,为字符串的总长度
            int stringWidth = 0;
            // 循环判断字符是不是全角字符
            foreach(char c in str)
            {
                // 是全角字符则加2,不是则加1
                stringWidth = stringWidth + (IsFullWidthCharacter(c) ? 2 : 1);
            }
            // 返回字符串长度
            return stringWidth;
        }



        // 判断是否为全角字符
        private static bool IsFullWidthCharacter(char c)
        {
            return (c >= 0x4E00 && c <= 0x9FFF) ||    // CJK统一表意文字
                   (c >= 0x3040 && c <= 0x309F) ||    // 日文平假名
                   (c >= 0x30A0 && c <= 0x30FF) ||    // 日文片假名
                   (c >= 0xAC00 && c <= 0xD7AF) ||    // 韩文字母
                   (c >= 0xFF00 && c <= 0xFFEF) ||    // 全角符号
                   (c >= 0x3000 && c <= 0x303F);      // CJK符号和标点
        }
    }
}
