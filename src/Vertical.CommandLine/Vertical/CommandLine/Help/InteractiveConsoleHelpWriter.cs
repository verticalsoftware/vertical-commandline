// Copyright(c) 2017 Vertical Software - All rights reserved
//
// This code file has been made available under the terms of the
// MIT license. Please refer to LICENSE.txt in the root directory
// or refer to https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
            "\tg, <           : First line",
            "\tG, >           : Next line",
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

        // Define key/command structure
        private struct CommandKeyMapping
        {
            internal CommandKeyMapping(Command command, char keyChar)
            {
                Command = command;
                KeyChar = keyChar;
            }

            internal Command Command { get; }
            internal char KeyChar { get; }
        }

        // Defines mapping between keys and commands
        private static readonly CommandKeyMapping[] Commands = new[]
        {
            new CommandKeyMapping(Command.PrevPage, 'b'),
            new CommandKeyMapping(Command.PrevHalfPage, 'u'),
            new CommandKeyMapping(Command.PrevLine, 'k'),
            new CommandKeyMapping(Command.NextPage, ' '),
            new CommandKeyMapping(Command.NextHalfPage, 'd'),
            new CommandKeyMapping(Command.NextLine, 'j'),
            new CommandKeyMapping(Command.NextLine, (char)0xd),
            new CommandKeyMapping(Command.NextLine, (char)0xa),
            new CommandKeyMapping(Command.FirstLine, 'g'),
            new CommandKeyMapping(Command.FirstLine, '<'),
            new CommandKeyMapping(Command.LastLine, 'G'),
            new CommandKeyMapping(Command.LastLine, '>'),
            new CommandKeyMapping(Command.Help, 'h'),
            new CommandKeyMapping(Command.Quit, 'q'),
            new CommandKeyMapping(Command.Quit, (char)0x1b)
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
                switch (PromptAndAwaitCommand(prompt))
                {
                    case Command.None:
                        if (_helpMode) return false;
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

            var keyChar = Console.ReadKey(true).KeyChar;

            return Array.Find(Commands, cmd => cmd.KeyChar == keyChar).Command;
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
