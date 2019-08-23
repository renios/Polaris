using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Dialogue
{
    public class DialogueParser
    {
        public static DialogueData ParseFromCSV(string path)
        {
            var asset = Resources.Load<TextAsset>(path);
            var rows = Regex.Split(asset.text, @"\r\n|\n\r|\n|\r");
            if (rows.Length < 1)
                return new DialogueData();

            var keys = Regex.Split(rows[0], @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
            var list = new List<Dictionary<string, object>>();
            for (int i = 1; i < rows.Length; i++)
            {
                var elements = Regex.Split(rows[i], @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))");
                if (elements.Length < 1 || elements[0] == "")
                    continue;

                var item = new Dictionary<string, object>();
                for (int j = 0; j < elements.Length; j++)
                {
                    var value = elements[j].TrimStart('\"').TrimEnd('\"').Replace("\\", "");
                    int n;
                    float f;
                    if (int.TryParse(value, out n))
                        item.Add(keys[j], n);
                    else if (float.TryParse(value, out f))
                        item.Add(keys[j], f);
                    else
                        item.Add(keys[j], value);
                }
                list.Add(item);
            }

            var contents = new Dictionary<int, List<DialogueContent>>();
            for (int i = 0; i < list.Count; i++)
            {
                int curPhase = (int)list[i]["phase"];
                if (!contents.ContainsKey(curPhase))
                    contents.Add(curPhase, new List<DialogueContent>());

                var newContent = new DialogueContent() { Type = -10 } ;
                switch(((string)list[i]["talker"]).ToLower())
                {
                    case "player":
                        newContent.Type = 1;
                        newContent.Talker = "주인공";
                        newContent.DialogText = (string)list[i]["dialogue"];
                        break;
                    case "*":
                        break;
                    default:
                        var imageKey = (string)list[i]["behaviour"];
                        if (imageKey != "-" && imageKey != "")
                        {
                            contents[curPhase].Add(new DialogueContent()
                            {
                                Type = 10,
                                ImageKey = imageKey
                            });
                        }
                        newContent.Type = 0;
                        newContent.Talker = (string)list[i]["talker"];
                        newContent.DialogText = (string)list[i]["dialogue"];
                        break;
                }
                if(newContent.Type == -10)
                {
                    switch(((string)list[i]["behaviour"]).ToLower())
                    {
                        case "select":
                            newContent.Type = 2;
                            newContent.JuncTexts = ((string)list[i]["dialogue"]).Split('@');
                            var nextList = new List<int>();
                            if (list[i]["nextPhases"] is int)
                                nextList.Add((int)list[i]["nextPhases"]);
                            else
                            {
                                var nextStr = ((string)list[i]["nextPhases"]).Split(',');
                                for (int j = 0; j < nextStr.Length; j++)
                                    nextList.Add(int.Parse(nextStr[j].Replace("`", "")));
                            }
                            newContent.Directions = nextList.ToArray();
                            break;
                        case "bgm":
                            newContent.Type = 11;
                            newContent.BgmKey = (string)list[i]["dialogue"];
                            break;
                        case "se":
                            newContent.Type = 12;
                            newContent.EffectKey = (string)list[i]["dialogue"];
                            break;
                        case "next":
                            newContent.Type = -1;
                            newContent.NextPhase = (int)list[i]["nextPhases"];
                            break;
                    }
                }
                if(newContent.Type == -10)
                {
                    Debug.Log("Parse failed at: path=" + path + ", line=" + (i + 1).ToString());
                    continue;
                }
                contents[curPhase].Add(newContent);
            }

            var dialog = new DialogueData();
            var phaseList = new List<DialoguePhase>();
            foreach (var phase in contents)
                phaseList.Add(new DialoguePhase() { Phase = phase.Key, Contents = phase.Value.ToArray() });
            dialog.Dialogues = phaseList.ToArray();
            return dialog;
        }
    }
}