using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui.Controls;
using WpfUiTest.Shared.Enums;
using WpfUiTest.Shared.Messages;

namespace WpfUiTest.Shared.Extensions
{
    // SnackBar消息扩展
    public static class SnackbarMessagingExtensions
    {
        // 显示主要（Primary）的SnackBar
        public static void ShowPrimary(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Primary,
                Icon = new SymbolIcon(icon ?? SymbolRegular.Branch24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }

        // 显示次要（Secondary）的SnackBar
        public static void ShowSecondary(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Secondary,
                Icon = new SymbolIcon(icon ?? SymbolRegular.BranchForkHint24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }

        // 显示信息（Info）的SnackBar
        public static void ShowInfo(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Info,
                Icon = new SymbolIcon(icon ?? SymbolRegular.Info24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }

        // 显示成功（Success）的SnackBar
        public static void ShowSuccess(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Success,
                Icon = new SymbolIcon(icon ?? SymbolRegular.CheckmarkCircle24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }

        // 显示警告（Caution）的SnackBar
        public static void ShowCaution(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Caution,
                Icon = new SymbolIcon(icon ?? SymbolRegular.Warning24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }

        // 显示错误（Danger）的SnackBar
        public static void ShowDanger(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Danger,
                Icon = new SymbolIcon(icon ?? SymbolRegular.DismissCircle24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }

        // 显示明亮（Light）的SnackBar
        public static void ShowLight(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Light,
                Icon = new SymbolIcon(icon ?? SymbolRegular.Lightbulb24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }

        // 显示暗黑（Dark）的SnackBar
        public static void ShowDark(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Dark,
                Icon = new SymbolIcon(icon ?? SymbolRegular.DarkTheme24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }

        // 显示透明（Transparent）的SnackBar
        public static void ShowTransparent(this IMessenger messenger, SnackbarTarget target, string title, string message, int timeout = 3, SymbolRegular? icon = null)
        {
            messenger.Send(new TargetedSnackbarMessage()
            {
                Target = target,
                Title = title,
                Message = message,
                Appearance = ControlAppearance.Transparent,
                Icon = new SymbolIcon(icon ?? SymbolRegular.BorderNone24),
                Timeout = TimeSpan.FromSeconds(timeout)
            }, target.ToString());
        }
    }
}
