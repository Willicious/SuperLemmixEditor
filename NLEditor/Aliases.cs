using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NLEditor
{
    enum AliasKind { Style, Gadget, Terrain, Background, Lemmings }

    struct Alias
    {
        public AliasKind Kind;
        public string From;
        public string To;
        public int Width;
        public int Height;
    }

    static class Aliases
    {
        static Aliases()
        {
            LoadStyleAliases("default");
        }

        private static readonly List<string> LoadedStyles = new List<string>();
        private static readonly List<Alias> Entries = new List<Alias>();

        public static Alias Dealias(string input, AliasKind kind)
        {
            Alias result = new Alias()
            {
                Kind = kind, // doesn't really matter
                From = input, // same
                To = input,
                Width = 0,
                Height = 0
            };

            if (string.IsNullOrEmpty(input))
                return result;

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

                        result.To = input;
                        // There won't be width / height info here. Or at least, shouldn't be.
                    }
                }

                if (kind != AliasKind.Style)
                {
                    foreach (var thisAlias in Entries.Where(ent => ent.Kind == kind))
                        if (thisAlias.From == input)
                        {
                            input = thisAlias.To;

                            result.To = input;
                            if (result.Width == 0) result.Width = thisAlias.Width;
                            if (result.Height == 0) result.Height = thisAlias.Height;
                        }
                }
            } while (input != lastInput);

            return result;
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
                            Width = entry["WIDTH"].ValueInt,
                            Height = entry["HEIGHT"].ValueInt,
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
