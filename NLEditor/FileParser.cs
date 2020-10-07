using System.Collections.Generic;
using System.IO;

namespace NLEditor
{
    /// <summary>
    /// Stores content of one line in a text file.
    /// </summary>
    class FileLine
    {
        /// <summary>
        /// Initializes a new instance of the NLEditor.FileLine class.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        /// <param name="num"></param>
        public FileLine(string key, string text = "", int num = 0)
        {
            if (key.StartsWith("$"))
            {
                Key = key.Substring(1).Trim().ToUpper();
                IsMultilineKey = true;
            }
            else
            {
                Key = key.ToUpper();
                IsMultilineKey = false;
            }
            Text = text;
            Value = num;
        }

        public string Key { get; private set; }
        public string Text { get; private set; }
        public int Value { get; private set; }
        public bool IsMultilineKey { get; private set; }
    }


    /// <summary>
    /// Can read from a text file and parse single lines or blocks of them. 
    /// </summary>
    class FileParser
    {
        /// <summary>
        /// Initializes a new instance of the NLEditor.FileParser class and opens the text file to be parsed. 
        /// <para> You have to catch exceptions in the method creating the FileParser. </para>
        /// </summary>
        /// <param name="filePath"></param>
        public FileParser(string filePath)
        {
            fileStream = new StreamReader(filePath);
        }

        /// <summary>
        /// Disposes the StreamReader and frees the file.
        /// </summary>
        public void DisposeStreamReader()
        {
            if (fileStream != null)
                fileStream.Dispose();
        }

        StreamReader fileStream;
        FileLine fileLatestLine;

        /// <summary>
        /// Parses new block of lines, ignoring empty lines or ones starting with #.
        /// <para> Returns null at the file end. </para>
        /// </summary>
        /// <returns></returns>
        public List<FileLine> GetNextLines()
        {
            var fileLineList = new List<FileLine>();

            FileLine curFileLine = null;
            if (fileLatestLine != null)
            {
                curFileLine = fileLatestLine;
                fileLatestLine = null;
            }

            do
            {
                curFileLine = GetNewLine();
                // end of file reached
                if (curFileLine == null)
                    return null;
            } while (curFileLine.Key == "");
            fileLineList.Add(curFileLine);

            // Add more lines, if the piece requires multiple lines in the level file
            if (curFileLine.IsMultilineKey)
            {
                bool doAddNextLine = true;
                while (doAddNextLine)
                {
                    curFileLine = GetNewLine();

                    if (curFileLine == null)
                    {
                        doAddNextLine = false;
                    }
                    else if (curFileLine.IsMultilineKey)
                    {
                        fileLatestLine = curFileLine;
                        doAddNextLine = false;
                    }
                    else if (curFileLine.Key != "")
                    {
                        fileLineList.Add(curFileLine);
                    }
                }
            }

            return fileLineList;
        }

        /// <summary>
        /// Reads a single line. Returns null at the file end.
        /// </summary>
        /// <returns></returns>
        private FileLine GetNewLine()
        {
            string line = fileStream.ReadLine();

            // Check for end of file
            if (line == null)
                return null;

            line = line.Trim();

            int seperatorIndex = line.IndexOf(' ');

            FileLine fileLine;
            if (line.Length > 0 && line.StartsWith("#"))
            {
                // comment line
                fileLine = new FileLine("");
            }
            else if (seperatorIndex == -1)
            {
                fileLine = new FileLine(line);
            }
            else
            {
                string key = line.Substring(0, seperatorIndex).Trim();
                string text = line.Substring(seperatorIndex).Trim();
                int value;
                if (!int.TryParse(text, out value))
                    value = 0;

                fileLine = new FileLine(key, text, value);
            }

            return fileLine;
        }

    }
}
