using UnityEngine;
using System.IO;
using System.Xml;
using System;
using System.Text;
using System.Collections;
using UnityEngine.UI;

public class XMLManager : MonoBehaviour 
{
    private static bool isFirst = true;
    private const string _xmlName = "GameData.xml";
    private static string _savePath;

    static void Init()
    {
        if (isFirst)
        {
            isFirst = false;
            if (Application.platform == RuntimePlatform.Android)
            {
                _savePath = Application.persistentDataPath + "/" + XMLName;//Android环境下的文件路径
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _savePath = Application.persistentDataPath + "/" + XMLName;//Android环境下的文件路径
            }
            else
            {
                _savePath = Application.dataPath + "/" + XMLName;//PC
            }
        }
    }


    private static string XMLName
    {
        get
        {
            if (isFirst)
            {
                Init();
            }
            return _xmlName;
        }
    }
    private static string SavePath
    {
        get
        {
            if (isFirst)
            {
                Init();
            }
            return _savePath;
        }
    }

    public static void SetString(string nowName,string newString)
    {
        XmlDocument xmlDoc = new XmlDocument();
        CreatXML();
        xmlDoc.Load(SavePath);
        XmlNode user = xmlDoc.SelectSingleNode("data/user");
        XmlNodeList nodeList = user.ChildNodes;
        bool isFind = false;
        foreach (XmlElement xe in nodeList)
        {
            if (xe.Name == nowName)
            {
                xe.InnerText = newString;
                isFind = true;
                break;
            }
        }
        if(!isFind)
        {
            XmlElement newXmlEle = xmlDoc.CreateElement(nowName);
            newXmlEle.InnerText = newString;
            user.AppendChild(newXmlEle);
        }
        xmlDoc.Save(SavePath);
    }


    public static string GetString(string nowName)
    {
        string newString = "";
        XmlDocument xmlDoc = new XmlDocument();
        CreatXML();
        xmlDoc.Load(SavePath);
        XmlNode user = xmlDoc.SelectSingleNode("data/user");
        XmlNodeList nodeList = user.ChildNodes;
        bool isFind = false;
        foreach (XmlElement xe in nodeList)
        {
            if (xe.Name == nowName)
            {
                newString = xe.InnerText;
                isFind = true;
                break;
            }
        }
        if (!isFind)
        {
            XmlElement newXmlEle = xmlDoc.CreateElement(nowName);
            newXmlEle.InnerText = newString.ToString();
            user.AppendChild(newXmlEle);
        }
        return newString;
    }

    private static void CreatXML()
    {
        //检测xml是否存在
        if(File.Exists(SavePath))
        {
            return;
        }
        //新建xml实例
        XmlDocument xmlDoc = new XmlDocument();
        //创建根节点，最上层节点
        XmlElement data = xmlDoc.CreateElement("data");
        xmlDoc.AppendChild(data);
        //二级节点
        XmlElement user = xmlDoc.CreateElement("user");
        data.AppendChild(user);
        /*//二级节点的属性
        XmlElement highScore = xmlDoc.CreateElement("highScore");
        highScore.InnerText = "0";
        user.AppendChild(highScore);*/
        xmlDoc.Save(SavePath);
    }
}