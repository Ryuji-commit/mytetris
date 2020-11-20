using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Collections;

namespace mytetris
{
    public class ParameterComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] original, int[] comparison)
        {
            if (original[0] == comparison[0] && original[1] == comparison[1])
            {
                return true;
            }
            return false;
        }
        public int GetHashCode(int[] i_obj)
        {
            return i_obj[0] ^ i_obj[1].GetHashCode();
        }
    }

    public partial class MyTetrisForm : Form
    {
        List<List<Rectangle>> board;
        int blockSideLength;
        int[][] blockPosition;
        List<int[]> stackedBlockPosition;
        public const int frameLimitX = 10;
        public const int frameLimitY = 20;

        public MyTetrisForm()
        {
            InitializeComponent();
            this.Height = 2 * this.Width;
            this.blockSideLength = this.Width / 18;
            this.board = new List<List<Rectangle>>();
            this.blockPosition = new int[][]
            {
                new int[] {3, 0},
                new int[] {3, 1},
                new int[] {3, 2},
            };
            this.stackedBlockPosition = new List<int[]>();

            for (int y = 0; y < MyTetrisForm.frameLimitY; y += 1)
            {
                List<Rectangle> line = new List<Rectangle>();
                for (int x = 0; x < MyTetrisForm.frameLimitX; x++)
                {
                    line.Add(new Rectangle(x * blockSideLength, y * blockSideLength, blockSideLength, blockSideLength));
                }
                this.board.Add(line);
            }

            Timer timer = new Timer
            {
                Interval = 1000
            };
            timer.Tick += new EventHandler(Update);
            timer.Start();
        }

        private void Update(object sender, EventArgs e)
        {
            ChangePointOfOperatingBlock(0, 1);
            Invalidate();
        }

        private void Draw(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 1);
            ParameterComparer comparer = new ParameterComparer();

            for (int y = 0; y < MyTetrisForm.frameLimitY; y += 1)
            {
                for (int x = 0; x < MyTetrisForm.frameLimitX; x += 1)
                {
                    int[] point = new int[] { x, y };
                    if (IsContainesPoint(point) || this.stackedBlockPosition.Contains(point, comparer))
                    {
                        e.Graphics.FillRectangle(Brushes.DeepSkyBlue, this.board[y][x]);
                        e.Graphics.DrawRectangle(blackPen, this.board[y][x]);
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(blackPen, this.board[y][x]);
                    }
                    
                }
            }
        }

        private void KeyInput(object sender, KeyEventArgs e)
        {
            int xMovingDistance = 0;
            int yMovingDistance = 0;
            switch (e.KeyData)
            {
                case Keys.Left:
                    xMovingDistance -= 1;
                    break;
                case Keys.Right:
                    xMovingDistance += 1; 
                    break;
                case Keys.Down:
                    yMovingDistance += 1;
                    break;
            }
            
            ChangePointOfOperatingBlock(xMovingDistance, yMovingDistance);
            Invalidate();
        }

        private bool IsContainesPoint(int[] point)
        {
            for (int oneDimensionalIndex = 0; oneDimensionalIndex < this.blockPosition.GetLength(0); oneDimensionalIndex++)
            {
                if (this.blockPosition[oneDimensionalIndex].SequenceEqual(point))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOperatingBlockInsideFrame(int[][] operatingBlockPosition, int yMovingDistance)
        {
            ParameterComparer comparer = new ParameterComparer();
            for (int oneDimensionalIndex = 0; oneDimensionalIndex < operatingBlockPosition.GetLength(0); oneDimensionalIndex++)
            {
                int[] sepBlockPosition = operatingBlockPosition[oneDimensionalIndex];
                if (sepBlockPosition[0] >= MyTetrisForm.frameLimitX || sepBlockPosition[0] < 0)
                {
                    return false;
                }

                if (sepBlockPosition[1] >= MyTetrisForm.frameLimitY)
                {
                    StackOperatingBlockPosition();
                    return false;
                }

                if (this.stackedBlockPosition.Contains(sepBlockPosition, comparer))
                {
                    if (yMovingDistance > 0) //縦方向の移動
                    {
                        StackOperatingBlockPosition();
                        return false;
                    }
                    else //横方向の移動
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void ChangePointOfOperatingBlock(int xMovingDistance,int yMovingDistance)
        {
            int[][] operatingBlockPosition = new int[this.blockPosition.GetLength(0)][];
            DeepCopyBlockPoint(operatingBlockPosition, this.blockPosition);

            for (int oneDimensionalIndex = 0; oneDimensionalIndex < operatingBlockPosition.GetLength(0); oneDimensionalIndex++)
            {
                operatingBlockPosition[oneDimensionalIndex][0] += xMovingDistance;
                operatingBlockPosition[oneDimensionalIndex][1] += yMovingDistance;
            }

            if (IsOperatingBlockInsideFrame(operatingBlockPosition, yMovingDistance))
            {
                DeepCopyBlockPoint(this.blockPosition, operatingBlockPosition);
            }
        }

        private void DeepCopyBlockPoint(int[][] copyBlockPosition, int[][] originalBlockPosition)
        {
            int oneDimensionalIndex;
            for (oneDimensionalIndex = 0; oneDimensionalIndex < copyBlockPosition.GetLength(0); oneDimensionalIndex++)
            {
                if (copyBlockPosition[oneDimensionalIndex] == null)
                {
                    copyBlockPosition[oneDimensionalIndex] = new int[2];
                }
            }

            for (oneDimensionalIndex = 0; oneDimensionalIndex < copyBlockPosition.GetLength(0); oneDimensionalIndex++)
            {
                copyBlockPosition[oneDimensionalIndex][0] = originalBlockPosition[oneDimensionalIndex][0];
                copyBlockPosition[oneDimensionalIndex][1] = originalBlockPosition[oneDimensionalIndex][1];
            }
        }

        private void StackOperatingBlockPosition()
        {
            for (int oneDimensionalIndex = 0; oneDimensionalIndex < this.blockPosition.GetLength(0); oneDimensionalIndex++)
            {
                this.stackedBlockPosition.Add(this.blockPosition[oneDimensionalIndex]);
            }
            EraseStackedBlocks();
            this.blockPosition = new int[][]
            {
                new int[] {3, 0},
                new int[] {3, 1},
                new int[] {3, 2},
            };
        }

        private void EraseStackedBlocks()
        {
            ParameterComparer comparer = new ParameterComparer();
            for (int indexY = MyTetrisForm.frameLimitY; indexY >= 0; indexY--)
            {
                for (int indexX = 0; indexX < MyTetrisForm.frameLimitX; indexX++)
                {
                    int[] point = new int[] { indexX, indexY };
                    if (!this.stackedBlockPosition.Contains(point, comparer))
                    {
                        break;
                    }
                    if ( indexX == MyTetrisForm.frameLimitX - 1 )
                    {
                        this.stackedBlockPosition = this.stackedBlockPosition.Where(sepBlockPoint => sepBlockPoint[1] != indexY).ToList();
                        Invalidate();
                    }
                }
            }
        }
    }
}
