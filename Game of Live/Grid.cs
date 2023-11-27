using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game_of_Live {
    public class Grid {
        public int Width { get; set; }
        public int Height { get; set; }
        private bool[,] grid { get; set; }
        public HashSet<Rectangle> Rectangles { get; set; }

        public Grid(int width, int height, HashSet<Rectangle> rectangles) {
            Width = width;
            Height = height;
            Rectangles = rectangles;
            grid = new bool[width, height];
        }

        public Grid CreateGrid(Rectangle[] rectangles) {
            int width = 0;
            int height = 0;
            HashSet<Rectangle> gridRectangles = new HashSet<Rectangle>();

            foreach (Rectangle rectangle in rectangles) {
                width = Math.Max(width, rectangle.Right);
                height = Math.Max(height, rectangle.Bottom);
                gridRectangles.Add(rectangle);
            }

            return new Grid(width, height, gridRectangles);
        }
    }
}