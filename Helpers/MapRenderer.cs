﻿/**
 *   Copyright (C) 2021 okaygo
 *
 *   https://github.com/misterokaygo/D2RAssist/
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <https://www.gnu.org/licenses/>.
 **/
using D2RAssist.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static D2RAssist.MapData;
using System.Text.RegularExpressions;

namespace D2RAssist.Helpers
{
    static class MapRenderer
    {
        public static Point LineEnd = new Point(0, 0);
        public static Bitmap CachedBackground;

        public static class Icons
        {
            public static readonly Bitmap DoorNext = CreateFilledRectangle(Settings.Map.Colors.DoorNext, 10, 10);
            public static readonly Bitmap DoorPrevious = CreateFilledRectangle(Settings.Map.Colors.DoorPrevious, 10, 10);
            public static readonly Bitmap Waypoint = CreateFilledRectangle(Settings.Map.Colors.Waypoint, 10, 10);
            public static readonly Bitmap Player = CreateFilledRectangle(Settings.Map.Colors.Player, 10, 10);
            public static readonly Bitmap SuperChest = CreateFilledEllipse(Settings.Map.Colors.SuperChest, 10, 10);
        }

        public static class MapPointsOfInterest
        {
            public static readonly string[] Chests = { "5", "6", "87", "104", "105", "106", "107", "143", "140", "141", "144", "146", "147", "148", "176", "177", "181", "183", "198", "240", "241", "242", "243", "329", "330", "331", "332", "333", "334", "335", "336", "354", "355", "356", "371", "387", "389", "390", "391", "397", "405", "406", "407", "413", "420", "424", "425", "430", "431", "432", "433", "454", "455", "501", "502", "504", "505", "580", "581", };
            public static readonly string[] Quests = { "357", "61", "376" };
            public static readonly string[] Waypoints = { "182", "298", "119", "145", "156", "157", "238", "237", "288", "323", "324", "398", "402", "429", "494", "496", "511", "539", "59", "60", "100" };
        }
            
        private static Bitmap CreateFilledRectangle(Color color, int width, int height)
        {
            Bitmap rectangle = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(rectangle);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillRectangle(new SolidBrush(color), 0, 0, width, height);
            graphics.Dispose();
            return rectangle;
        }

        private static Bitmap CreateFilledEllipse(Color color, int width, int height)
        {
            Bitmap ellipse = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(ellipse);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillEllipse(new SolidBrush(color), 0, 0, width, height);
            graphics.Dispose();
            return ellipse;
        }

        public static void Clear()
        {
            CachedBackground = null;
            LineEnd = new Point(0, 0);
        }

        public static Bitmap FromMapData(MapData mapData)
        {

            Graphics CachedBackgroundGraphics;
            Bitmap updatedMap;

            if (CachedBackground == null)
            {
                var uncroppedBackground = new Bitmap(mapData.mapRows[0].Length, mapData.mapRows.Length, PixelFormat.Format32bppArgb);

                for (int x = 0; x < mapData.mapRows.Length; x++)
                {
                    for (int y = 0; y < mapData.mapRows[x].Length; y++)
                    {
                        int type = mapData.mapRows[x][y];
                        switch (type)
                        {
                            case 1:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(70, 51, 41));
                                break;
                            case -1:
                                // uncroppedBackground.SetPixel(y, x, Color.FromArgb(255, 255, 255));
                                break;
                            case 0:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(0, 0, 0));
                                break;
                            case 16:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(168, 56, 50));
                                break;
                            case 7:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(255, 255, 255));
                                break;
                            case 5:
                                // uncroppedBackground.SetPixel(y, x, Color.FromArgb(0, 0, 0));
                                break;
                            case 33:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(0, 0, 255));
                                break;
                            case 23:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(0, 0, 255));
                                break;
                            case 4:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(0, 255, 255));
                                break;
                            case 21:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(255, 0, 255));
                                break;
                            case 20:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(70, 51, 41));
                                break;
                            case 17:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(255, 51, 255));
                                break;
                            case 3:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(255, 0, 255));
                                break;
                            case 19:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(0, 51, 255));
                                break;
                            case 2:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(10, 51, 23));
                                break;
                            case 37:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(50, 51, 23));
                                break;
                            case 6:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(80, 51, 33));
                                break;
                            case 39:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(20, 11, 33));
                                break;
                            case 53:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(10, 11, 43));
                                break;
                            default:
                                uncroppedBackground.SetPixel(y, x, Color.FromArgb(255, 255, 255));
                                break;
                        }
                    }
                }
                CachedBackground = ImageManipulation.CropBitmap(uncroppedBackground);
            }

            double biggestDimension = CachedBackground.Width > CachedBackground.Height ? CachedBackground.Width : CachedBackground.Height;

            double multiplier = Settings.Map.Size / biggestDimension;

            if (multiplier == 0)
            {
                multiplier = 1;
            }

            int miniMapPlayerX = (int)((Globals.CurrentGameData.PlayerX - mapData.levelOrigin.x) * multiplier);
            int miniMapPlayerY = (int)((Globals.CurrentGameData.PlayerY - mapData.levelOrigin.y) * multiplier);
            Point playerPoint = new Point(miniMapPlayerX, miniMapPlayerY);
            updatedMap = ImageManipulation.ResizeImage((Bitmap)CachedBackground.Clone(), (int)(CachedBackground.Width * multiplier), (int)(CachedBackground.Height * multiplier));
            CachedBackgroundGraphics = Graphics.FromImage(updatedMap);
            DrawArrows(miniMapPlayerX, miniMapPlayerY, CachedBackgroundGraphics, multiplier, mapData);

            int counter = 0;
            int originX = mapData.levelOrigin.x;
            int originY = mapData.levelOrigin.y;

            foreach (KeyValuePair<string, AdjacentLevel> i in mapData.adjacentLevels)
            {
                if (mapData.adjacentLevels[i.Key].exits.Length == 0)
                {
                    continue;
                }
                int xnew = mapData.adjacentLevels[i.Key].exits[0].x;
                int ynew = mapData.adjacentLevels[i.Key].exits[0].y;
                int xcoord = (int)(multiplier * (xnew - originX));
                int ycoord = (int)(multiplier * (ynew - originY));
                var nextLevelPoint = LineEnd = new Point(xcoord, ycoord);
                if (counter == 0)
                {
                    CachedBackgroundGraphics.DrawImage(Icons.DoorPrevious, nextLevelPoint);
                }
                else
                {
                    CachedBackgroundGraphics.DrawImage(Icons.DoorNext, nextLevelPoint);
                }
                counter++;
            }

            foreach (KeyValuePair<string, XY[]> mapObject in mapData.objects)
            {
                int xCoord = MultiplyIntByDouble(mapData.objects[mapObject.Key][0].x - originX, multiplier);
                int yCoord = MultiplyIntByDouble(mapData.objects[mapObject.Key][0].y - originY, multiplier);
                Point mapObjectPoint = new Point(xCoord, yCoord);
                if ((string)mapObject.Key == "182" || (string)mapObject.Key == "298" || (string)mapObject.Key == "119" || (string)mapObject.Key == "145" || (string)mapObject.Key == "156" || (string)mapObject.Key == "157" || (string)mapObject.Key == "238" || (string)mapObject.Key == "237" || (string)mapObject.Key == "288" || (string)mapObject.Key == "323" || (string)mapObject.Key == "324" || (string)mapObject.Key == "398" || (string)mapObject.Key == "402" || (string)mapObject.Key == "429" || (string)mapObject.Key == "494" || (string)mapObject.Key == "496" || (string)mapObject.Key == "511" || (string)mapObject.Key == "539" || (string)mapObject.Key == "59" || (string)mapObject.Key == "60" || (string)mapObject.Key == "100")
                {
                    CachedBackgroundGraphics.DrawImage(Icons.Waypoint, mapObjectPoint);
                }
                else if ((string)mapObject.Key == "152" || (string)mapObject.Key == "357" || (string)mapObject.Key == "356" || (string)mapObject.Key == "354" || (string)mapObject.Key == "355" || (string)mapObject.Key == "266")
                {
                    CachedBackgroundGraphics.DrawImage(Icons.DoorNext, mapObjectPoint);
                }
                else if (MapPointsOfInterest.Chests.Contains((string)mapObject.Key))
                {
                    foreach (XY coordinates in mapData.objects[mapObject.Key])
                    {
                        xCoord = MultiplyIntByDouble(coordinates.x - originX, multiplier);
                        yCoord = MultiplyIntByDouble(coordinates.y - originY, multiplier);
                        mapObjectPoint = new Point(xCoord, yCoord);
                        CachedBackgroundGraphics.DrawImage(Icons.SuperChest, mapObjectPoint);
                    }
                }
            }

            CachedBackgroundGraphics.DrawImage(Icons.Player, playerPoint);

            if (Settings.Map.Rotate)
            {
                updatedMap = ImageManipulation.RotateImage(updatedMap, 53, true, false, Color.Transparent);
            }

            return updatedMap;
        }

        private static int MultiplyIntByDouble(int _int, double _double)
        {
            return (int)(_int * _double);
        }

        private static void DrawArrows(int miniMapPlayerX, int miniMapPlayerY, Graphics minimap, double multiplier, MapData mapData)
        {
            //exits
            foreach (KeyValuePair<string, AdjacentLevel> i in mapData.adjacentLevels)
            {
                if (mapData.adjacentLevels[i.Key].exits.Length == 0)
                {
                    continue;
                }
                int originX = mapData.levelOrigin.x;
                int originY = mapData.levelOrigin.y;
                int xnew = mapData.adjacentLevels[i.Key].exits[0].x;
                int ynew = mapData.adjacentLevels[i.Key].exits[0].y;
                int xcoord = (int)((xnew - originX) * multiplier);
                int ycoord = (int)((ynew - originY) * multiplier);

                //draw arrow
                AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
                Pen p = new Pen(Settings.Map.Colors.ArrowExit, 5);
                p.CustomEndCap = bigArrow;
                minimap.DrawLine(p, miniMapPlayerX, miniMapPlayerY, xcoord, ycoord);
                //draw label
                DrawLabelAt("Area", i.Key, xcoord, ycoord, minimap);
            }
            
            bool wpFound = false;
            foreach (KeyValuePair<string, XY[]> mapObject in mapData.objects)
            {
                if (!wpFound && MapPointsOfInterest.Waypoints.Contains((string)mapObject.Key))
                {
                    int originX = mapData.levelOrigin.x;
                    int originY = mapData.levelOrigin.y;
                    int pointX = (int)((mapData.objects[mapObject.Key][0].x - originX) * multiplier);
                    int pointY = (int)((mapData.objects[mapObject.Key][0].y - originY) * multiplier);
                    //draw arrow
                    AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
                    Pen p = new Pen(Settings.Map.Colors.ArrowWaypoint, 5);
                    p.CustomEndCap = bigArrow;

                    minimap.DrawLine(p, miniMapPlayerX, miniMapPlayerY, pointX, pointY);
                    //draw label
                    DrawLabelAt("GameObject", mapObject.Key, pointX, pointY, minimap);
                    wpFound = true;
                }
                if (MapPointsOfInterest.Quests.Contains((string)mapObject.Key))
                {
                    int originX = mapData.levelOrigin.x;
                    int originY = mapData.levelOrigin.y;
                    int pointX = (int)((mapData.objects[mapObject.Key][0].x - originX) * multiplier);
                    int pointY = (int)((mapData.objects[mapObject.Key][0].y - originY) * multiplier);
                    //draw arrow
                    AdjustableArrowCap bigArrow = new AdjustableArrowCap(5, 5);
                    Pen p = new Pen(Settings.Map.Colors.ArrowQuest, 5);
                    p.CustomEndCap = bigArrow;
                    minimap.DrawLine(p, miniMapPlayerX, miniMapPlayerY, pointX, pointY);
                    //draw label
                    DrawLabelAt("GameObject", mapObject.Key, pointX, pointY, minimap);
                }
            }
        }
        private static void DrawLabelAt(string objectType, string objectKey, int posx, int posy, Graphics minimap)
        {
            minimap.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            // Create font and brush.
            Font drawFont = new Font(Settings.Map.LabelFont, 10);
            SolidBrush drawBrush = new SolidBrush(Settings.Map.Colors.LabelColor);
            // Set format of string.
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.NoClip;
            //draw label line
            int adjustment = 20;
            Pen p = new Pen(Settings.Map.Colors.LabelColor, 1);
            minimap.DrawLine(p, posx + adjustment, posy - (adjustment), posx, posy);
            //label box
            RectangleF rectF1 = new RectangleF(posx + adjustment, posy - (2 * adjustment), 100, 100);
            if (objectType == "Area")
            {
                string objName = Enum.GetName(typeof(Game.Area), Int32.Parse(objectKey));
                objName = Game.AreaName[Int32.Parse(objectKey)];
                minimap.DrawString(objName, drawFont, drawBrush, rectF1, drawFormat);
            }
            if (objectType == "GameObject")
            {
                string objName = Enum.GetName(typeof(Game.GameObject), Int32.Parse(objectKey));
                if (objName.Contains("Waypoint")) objName = "Waypoint";
                minimap.DrawString(objName, drawFont, drawBrush, rectF1, drawFormat);
            }
        }
    }
}