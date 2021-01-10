﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NLEditor
{
    enum AliasKind { Style, Gadget, Terrain, Background, Lemmings }

    static class Aliases
    {
        static Aliases()
        {
            LoadStyleAliases("default");
        }

        private struct Alias
        {
            public AliasKind Kind;
            public string From;
            public string To;
        }

        private static readonly List<string> LoadedStyles = new List<string>();
        private static readonly List<Alias> Entries = new List<Alias>();

        public static string Dealias(string input, AliasKind kind)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            string lastInput;
            do
            {
                lastInput = input;

                LoadStyleAliases(input.Split(':')[0]);

                foreach (var thisAlias in Entries.Where(ent => ent.Kind == AliasKind.Style))
                {
                    if (thisAlias.From == input.Split(':')[0])
                    {
                        string[] splitInput = input.Split(':');

                        if (splitInput.Length == 1)
                            input = thisAlias.To;
                        else
                            input = thisAlias.To + ":" + splitInput[1];
                    }
                }

                if (kind != AliasKind.Style)
                {
                    foreach (var thisAlias in Entries.Where(ent => ent.Kind == kind))
                        if (thisAlias.From == input)
                            input = thisAlias.To;
                }
            } while (input != lastInput);

            return input;
        }

        public static void LoadStyleAliases(string style)
        {
            if (!LoadedStyles.Contains(style))
            {
                LoadedStyles.Add(style);

                if (File.Exists(C.AppPathPieces + style + C.DirSep + "alias.nxmi"))
                {
                    NLTextDataNode aliasFile = NLTextParser.LoadFile(C.AppPathPieces + style + C.DirSep + "alias.nxmi");

                    foreach (NLTextDataNode entry in aliasFile.Children)
                    {
                        AliasKind kind;
                        switch (entry.Key)
                        {
                            case "GADGET":
                                kind = AliasKind.Gadget;
                                break;
                            case "TERRAIN":
                                kind = AliasKind.Terrain;
                                break;
                            case "BACKGROUND":
                                kind = AliasKind.Background;
                                break;
                            case "LEMMINGS":
                                kind = AliasKind.Lemmings;
                                break;
                            case "STYLE":
                                kind = AliasKind.Style;
                                break;
                            default:
                                continue;
                        }

                        Alias newAlias = new Alias()
                        {
                            From = entry["FROM"].Value,
                            To = entry["TO"].Value,
                            Kind = kind
                        };

                        if (newAlias.From[0] == ':')
                            newAlias.From = style + newAlias.From;

                        if (newAlias.To[0] == ':')
                            newAlias.To = style + newAlias.To;

                        Entries.Add(newAlias);
                    }
                }
            }
        }
    }
}
