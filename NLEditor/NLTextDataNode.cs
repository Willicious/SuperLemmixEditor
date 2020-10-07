using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace NLEditor
{
    static class NLTextParser
    {
        public static NLTextDataNode LoadFile(string filename)
        {
            return LoadStrings(File.ReadAllText(filename).Replace("\r\n", "\n").Replace("\r", "\n").Split('\n'));
        }

        public static NLTextDataNode LoadStrings(string[] lines)
        {
            List<NLTextDataNode> stack = new List<NLTextDataNode>();
            NLTextDataNode result = new NLTextDataNode();

            stack.Add(result);

            foreach (string rawLine in lines)
            {
                string line = rawLine.TrimStart();

                if (line == "")
                    continue;

                if (line[0] == '#')
                    continue;

                if (line.Trim().ToUpperInvariant() == "$END")
                {
                    stack.RemoveAt(stack.Count - 1);
                    continue;
                }

                bool isSection = (line[0] == '$');
                int sepPos = line.IndexOf(' ');

                NLTextDataNode newChild = new NLTextDataNode();
                stack[stack.Count - 1].AddChild(newChild);

                if (sepPos < 0)
                {
                    newChild.Key = line;
                }
                else
                {
                    newChild.Key = line.Substring(0, sepPos);
                    newChild.Value = line.Substring(sepPos + 1);
                }

                if (isSection)
                {
                    newChild.Key = newChild.Key.Substring(1);
                    stack.Add(newChild);
                }
            }

            return result;
        }
    }

    class NLTextDataNode : IEnumerable<NLTextDataNode>
    {
        public readonly List<NLTextDataNode> Children = new List<NLTextDataNode>();

        public void AddChild(NLTextDataNode node)
        {
            Children.Add(node);
        }

        private string _Key = "";
        public string Key
        {
            get
            {
                return _Key;
            }

            set
            {
                if (value == null)
                    _Key = "";
                else
                    _Key = value.Trim().ToUpperInvariant();
            }
        }

        private string _Value = "";

        public string Value
        {
            get
            {
                return _Value;
            }

            set
            {
                _Value = value ?? "";
            }
        }

        public string ValueTrimUpper { get => Value.Trim().ToUpperInvariant(); }

        public int ValueInt
        {
            get
            {
                int result;

                if (_Value != "" && (_Value[0] == 'X' || _Value[0] == 'x'))
                    int.TryParse(_Value.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
                else
                    int.TryParse(_Value, out result);

                return result;
            }

            set
            {
                Value = value.ToString();
            }
        }

        public ulong ValueUInt64
        {
            get
            {
                ulong result;

                if (_Value != "" && (_Value[0] == 'X' || _Value[0] == 'x'))
                    ulong.TryParse(_Value.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
                else
                    ulong.TryParse(_Value, out result);

                return result;
            }

            set
            {
                Value = value.ToString();
            }
        }

        public void SetValueHex(int value, int digits)
        {
            _Value = value.ToString("X" + digits.ToString());
        }

        public void SetValueHex(ulong value, int digits)
        {
            _Value = value.ToString("X" + digits.ToString());
        }

        public bool HasChildWithKey(string key)
        {
            return Children.Count(child => child.Key == key.Trim().ToUpperInvariant()) > 0;
        }

        public NLTextDataNode this[string key]
        {
            get
            {
                NLTextDataNode result = Children.LastOrDefault(child => child.Key == key.Trim().ToUpperInvariant());

                if (result == null)
                {
                    result = new NLTextDataNode();
                    result.Key = key;
                    AddChild(result);
                }

                return result;
            }
        }

        public IEnumerator<NLTextDataNode> GetEnumerator()
        {
            foreach (var item in Children)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in Children)
                yield return item;
        }
    }
}
