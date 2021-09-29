// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Vertical.CommandLine.Help
{
    /// <summary>
    /// Represents a help text writer that provides an interactive environment for
    /// paging.
    /// </summary>
    [ExcludeFromCodeCoverage] // Manually test since it relies on console input & output
    public sealed class InteractiveConsoleHelpWriter : IHelpWriter
    {
        private readonly bool _helpMode;
        
        private InteractiveConsoleHelpWriter(bool helpMode)
        {
            _helpMode = helpMode;
        }

        /// <summary>
        /// Defines a default instance.
        /// </summary>
        public static IHelpWriter Default { get; } = new InteractiveConsoleHelpWriter(helpMode: false);

        // Defines help content.
        private static readonly string[] HelpContent =
        {
            "DESCRIPTION",
            "\tThe interactive help viewer is a 'less' like utility commonly found on *nix systems. You may navigate "+
                "the help content of the application you are using by pressing one of the key commands listed below. ",
            "",
            "COMMANDS",
            "\tSpace bar      : Next page",
            "\td              : Next half page",
            "\tb              : Previous page",
            "\tu              : Previous half page",
            "\tj, <enter>     : Next line",
            "\tk              : Previous line",
            "\tg, <           : First page",
            "\tG, >           : Last page",
            "\th              : Show command list",
            "\tq              : Quit"
        };

        // Represents an object that inverts console colors.
        private class InvertedConsoleColors : IDisposable
        {
            private readonly ConsoleColor _foregroundColor;
            private readonly ConsoleColor _backgroundColor;

            internal InvertedConsoleColors()
            {
                _foregroundColor = Console.ForegroundColor;
                _backgroundColor = Console.BackgroundColor;
                Console.ForegroundColor = Console.BackgroundColor;
                Console.BackgroundColor = _foregroundColor;
            }

            public void Dispose()
            {
                Console.ForegroundColor = _foregroundColor;
                Console.BackgroundColor = _backgroundColor;
            }
        }

        // Defines the navigation commands.
        private enum Command
        {
            None,
            PrevPage,
            PrevHalfPage,
            PrevLine,
            NextPage,
            NextHalfPage,
            NextLine,
            FirstLine,
            LastLine,
            Help,
            Quit
        }
        
        private sealed class CommandKeyMapping2
        {
            private readonly ConsoleKey _key;
            private readonly ConsoleModifiers _modifiers;
            
            internal CommandKeyMapping2(Command command, ConsoleKey key, bool shift = false)
            {
                Command = command;
                
                _key = key;
                _modifiers = shift ? ConsoleModifiers.Shift : default;
            }
            
            internal Command Command { get; }

            internal bool IsEquivalentOf(in ConsoleKeyInfo key) => key.Key == _key && key.Modifiers == _modifiers;
        }

        // Defines mapping between keys and commands
        private static readonly CommandKeyMapping2[] Commands = new[]
        {
            new CommandKeyMapping2(Command.PrevPage, ConsoleKey.B),
            new CommandKeyMapping2(Command.PrevPage, ConsoleKey.UpArrow),
            new CommandKeyMapping2(Command.PrevHalfPage, ConsoleKey.U),
            new CommandKeyMapping2(Command.PrevLine, ConsoleKey.K),
            new CommandKeyMapping2(Command.NextPage, ConsoleKey.Spacebar),
            new CommandKeyMapping2(Command.NextPage, ConsoleKey.DownArrow),
            new CommandKeyMapping2(Command.NextHalfPage, ConsoleKey.D),
            new CommandKeyMapping2(Command.NextLine, ConsoleKey.J),
            new CommandKeyMapping2(Command.NextLine, ConsoleKey.Enter),
            new CommandKeyMapping2(Command.FirstLine, ConsoleKey.G),
            new CommandKeyMapping2(Command.FirstLine, ConsoleKey.OemComma, shift: true),
            new CommandKeyMapping2(Command.LastLine, ConsoleKey.G, shift: true),
            new CommandKeyMapping2(Command.LastLine, ConsoleKey.OemPeriod, shift: true),
            new CommandKeyMapping2(Command.Help, ConsoleKey.H),
            new CommandKeyMapping2(Command.Quit, ConsoleKey.Q),
            new CommandKeyMapping2(Command.Quit, ConsoleKey.Escape)
        };
        
        /// <inheritdoc />
        public void WriteContent(IReadOnlyCollection<string> content)
        {
            ClearConsoleInput();
            EnterInteractiveMode(content);

            if (!_helpMode) Console.WriteLine();
        }

        private void EnterInteractiveMode(IReadOnlyCollection<string> content)
        {
            FormatInfo formatInfo;
            var startRow = 0;
            var lineCount = 0;

            do
            {
                formatInfo = new FormatInfo(Console.WindowWidth - 5, Console.WindowHeight - 2, startRow);
                Console.Clear();
                lineCount = HelpWriter.WriteContent(Console.Out, content, formatInfo);

            } while (HandleUserInput(formatInfo, lineCount, out startRow));

            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        private bool HandleUserInput(FormatInfo formatInfo, int lineCount, out int startRow)
        {
            var prompt = _helpMode ? "[any key]" : string.Empty;
            startRow = formatInfo.StartRow;

            while (true)
            {
                var command = PromptAndAwaitCommand(prompt);

                if (_helpMode) return false;
                
                switch (command)
                {
                    case Command.None:
                        prompt = "[command], [h]elp, [q]uit";
                        continue;

                    case Command.Quit:
                        return false;

                    case Command.PrevLine:
                        startRow--;
                        break;

                    case Command.PrevHalfPage:
                        startRow -= formatInfo.FormatHeight / 2;
                        break;

                    case Command.PrevPage:
                        startRow -= formatInfo.FormatHeight;
                        break;

                    case Command.NextLine:
                        startRow++;
                        break;

                    case Command.NextHalfPage:
                        startRow += formatInfo.FormatHeight / 2;
                        break;

                    case Command.NextPage:
                        startRow += formatInfo.FormatHeight;
                        break;

                    case Command.FirstLine:
                        startRow = 0;
                        break;

                    case Command.LastLine:
                        startRow = int.MaxValue;
                        break;

                    case Command.Help:
                        ShowHelp();
                        return true;
                }

                startRow = Math.Max(0, Math.Min(startRow, lineCount - formatInfo.FormatHeight));
                return true;
            }
        }

        private static Command PromptAndAwaitCommand(string prompt)
        {
            using (new InvertedConsoleColors())
            {
                Console.SetCursorPosition(0, Console.WindowHeight - 1);
                Console.Write(prompt);
                Console.Write(":");
            }
            Console.Write(" ");

            var keyInfo = Console.ReadKey(true);

            return Commands.FirstOrDefault(cmd => cmd.IsEquivalentOf(keyInfo))?.Command
                   ?? Command.None;
        }

        private static void ShowHelp()
        {
            var interactiveViewer = new InteractiveConsoleHelpWriter(helpMode: true);
            interactiveViewer.WriteContent(HelpContent);
        }

        private static void ClearConsoleInput()
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }
    }
}
