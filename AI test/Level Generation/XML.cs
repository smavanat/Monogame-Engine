using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace AI_test.Core
{
    //Used for manipulating stuff in XML files, but since I only need simple stuff, I've mostly moved that to the Grid class
    static class WriteToXML
    {
        const string FILENAME = @"C:\Users\EdwinH\Documents\My Work\Data Structures and Algorithms\AI test\AI test\Level Generation\Levels\Test Level.xml";
        //Writes a 2D array to an XML file
        public static void WriteGridToXML(Node[,] grid)
        {
            string xml = $"<{nameof(grid)}></{nameof(grid)}>";

            XDocument doc = XDocument.Parse(xml);
            XElement data = doc.Root;

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                XElement xRow = new XElement("Row");
                data.Add(xRow);
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    XElement xCol = new XElement("Column", grid[row, col].bitValue);
                    xRow.Add(xCol);
                }
            }

            doc.Save(FILENAME);
        }
        //Changes a value at a specified index of a 2D array in an XML file
        public static void ChangeXMLValueinGrid(string value, int rowIndex, int columnIndex)
        {
            XDocument xmlFile = XDocument.Load(FILENAME);
            var element = xmlFile.Elements("grid").Descendants("Row").ElementAt(rowIndex).Descendants("Column").ElementAt(columnIndex);

            element.Value = value;

            xmlFile.Save(FILENAME);
        }
        //Returns a 2D array from a set of values in an XML file
        public static DataTable LoadDataTableFromXML(string Path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);
            DataSet dataSet = new DataSet();
            dataSet.ReadXml(new StringReader(doc.InnerXml));
            return dataSet.Tables[0];
        }

        public static Node[,] GetGridFromXML(Node[,] grid)
        {
            XDocument xmlFile = XDocument.Load(FILENAME);
            XElement root = xmlFile.Root;
            var attributeX = root.Element("sizes").Element("sizeX");
            var attributeY = root.Element("sizes").Element("sizeY");
            string layout = root.Element("layout").Value;

            int sizeX = Int32.Parse(attributeX.Value);
            int sizeY = Int32.Parse(attributeY.Value);
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (j + 1 % sizeY == 0)
                    {
                        continue;
                    }
                    else
                    {
                        int imageWidth;
                        if (Int32.TryParse(layout[(sizeX * j) + i].ToString(), out imageWidth))
                        {
                            grid[i, j].bitValue = imageWidth;
                        }
                        //grid[i, j].bitValue = Int32.TryParse(layout[(sizeX * i) + j].ToString());
                    }
                }
            }
            return grid;
        }
        
    }
    
}
