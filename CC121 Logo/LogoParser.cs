using System.Collections.Generic;

namespace Logo
{
    class LogoParser
    {
        private string my_Input = "";
        private List<LogoCommand> my_Commands = new List<LogoCommand>();

        public string Input
        {
            get { return my_Input; }
            set { my_Input = value; }
        }

        public List<LogoCommand> Commands
        {
            get { return my_Commands; }
        }

        public void Parse()
        {
            my_Commands = new List<LogoCommand>();
            Parse(my_Input);
        }

        private void Parse(string txt)
        {
            LogoCommand cmd;
            int index = 0;
            string substring;
            string my_Txt = RemoveSpaces(txt);
            while (index < my_Txt.Length)
            {
                if (index < my_Txt.Length - 6)
                {
                    if (my_Txt.Substring(index, 6) == "repeat")
                    {
                        double value = GetValue(my_Txt, index + 6);
                        if (value >= 0)
                        {
                            string parameter = GetParameter(my_Txt, index + 6 + value.ToString().Length);
                            if (parameter != "")
                            {
                                index += value.ToString().Length + parameter.Length + 5;
                                for (int I = 0; I < value; I++)
                                {
                                    Parse(parameter);
                                }
                            }
                        }
                    }
                }
                if (index < my_Txt.Length - 4)
                {
                    substring = my_Txt.Substring(index, 4);
                    if (substring == "setX")
                    {
                        int value = GetValue(my_Txt, index + 4);
                        if (value >= 0)
                        {
                            cmd = new LogoCommand()
                            {
                                Command = "setX",
                                Value = value
                            };
                            my_Commands.Add(cmd);
                            index += value.ToString().Length + 3;
                        }
                    }
                    else if (substring == "setY")
                    {
                        int value = GetValue(my_Txt, index + 4);
                        if (value >= 0)
                        {
                            cmd = new LogoCommand()
                            {
                                Command = "setY",
                                Value = value
                            };
                            my_Commands.Add(cmd);
                            index += value.ToString().Length + 3;
                        }
                    }
                    else if (substring == "size")
                    {
                        int value = GetValue(my_Txt, index + 4);
                        if (value >= 0)
                        {
                            cmd = new LogoCommand()
                            {
                                Command = "size",
                                Value = value
                            };
                            my_Commands.Add(cmd);
                            index += value.ToString().Length + 3;
                        }
                    }
                    else if (substring == "home")
                    {
                        cmd = new LogoCommand()
                        {
                            Command = "home",
                            Value = 0
                        };
                        my_Commands.Add(cmd);
                        index += 3;
                    }
                }
                if (index < my_Txt.Length - 3)
                {
                    if (my_Txt.Substring(index, 3) == "col")
                    {
                        string parameter = GetParameter(my_Txt, index + 3);
                        if (parameter != "")
                        {
                            cmd = new LogoCommand()
                            {
                                Command = "col",
                                Value = 0,
                                Parameter = parameter
                            };
                            my_Commands.Add(cmd);
                            index += parameter.Length + 3;
                        }
                    }
                }
                if (index < my_Txt.Length - 2)
                {
                    substring = my_Txt.Substring(index, 2);
                    if (substring == "fd")
                    {
                        int value = GetValue(my_Txt, index + 2);
                        if (value >= 0)
                        {
                            cmd = new LogoCommand()
                            {
                                Command = "fd",
                                Value = value
                            };
                            my_Commands.Add(cmd);
                            index += value.ToString().Length + 1;
                        }
                    }
                    else if (substring == "bd")
                    {
                        int value = GetValue(my_Txt, index + 2);
                        if (value >= 0)
                        {
                            cmd = new LogoCommand()
                            {
                                Command = "bd",
                                Value = value
                            };
                            my_Commands.Add(cmd);
                            index += value.ToString().Length + 1;
                        }
                    }
                    else if (substring == "rt")
                    {
                        int value = GetValue(my_Txt, index + 2);
                        if (value >= 0)
                        {
                            cmd = new LogoCommand()
                            {
                                Command = "rt",
                                Value = value
                            };
                            my_Commands.Add(cmd);
                            index += value.ToString().Length + 1;
                        }
                    }
                    else if (substring == "lt")
                    {
                        int value = GetValue(my_Txt, index + 2);
                        if (value >= 0)
                        {
                            cmd = new LogoCommand()
                            {
                                Command = "lt",
                                Value = value
                            };
                            my_Commands.Add(cmd);
                            index += value.ToString().Length + 1;
                        }
                    }
                    else if (substring == "pu")
                    {
                        cmd = new LogoCommand()
                        {
                            Command = "pu",
                            Value = 0
                        };
                        my_Commands.Add(cmd);
                        index += 1;
                    }
                    else if (substring == "pd")
                    {
                        cmd = new LogoCommand()
                        {
                            Command = "pd",
                            Value = 0
                        };
                        my_Commands.Add(cmd);
                        index += 1;
                    }
                }
                index += 1;
            }
        }

        private string RemoveSpaces(string txt)
        {
            string result = "";
            int index = 0;
            while (index < txt.Length)
            {
                if (txt[index] != ' ') result += txt[index]; //Skip spaces
                index += 1;
            }
            return result;
        }

        private int GetValue(string txt, int index)
        {
            int result = -1;
            int I = 1;
            while (index + I <= txt.Length)
            {
                if (int.TryParse(txt.Substring(index, I), out int test))
                {
                    result = test;
                    I++;
                }
                else
                {
                    return result;
                }
            }
            return result;
        }

        private string GetParameter(string txt, int index)
        {
            int bracketcounter;
            string result = "";
            int I = index;
            if (index < txt.Length)
            {
                if (txt[I] != '[') return "";
                I += 1;
                bracketcounter = 1;
                while (I < txt.Length)
                {
                    if (txt[I] == '[')
                    {
                        bracketcounter += 1;
                    }
                    else if (txt[I] == ']')
                    {
                        bracketcounter -= 1;
                        if (bracketcounter == 0) return result;
                    }
                    result += txt[I];
                    I += 1;
                }
            }
            return "";
        }
    }
}
