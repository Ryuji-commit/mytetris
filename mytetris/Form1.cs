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
    public partial class MyTetrisForm : Form
    {
        List<List<Rectangle>> board;
        int blockSideLength;
        int[][] blockPosition;
        List<int[]> stackedBlockPosition;
        List<List<Brush>> brushList;
        Brush myBlockBrush;

        public const int frameLimitX = 10;
        public const int frameLimitY = 20;

        public int score;

        public MyTetrisForm()
        {
            InitializeComponent();
            this.Height = 2 * this.Width;
            this.blockSideLength = this.Width / 18;
            this.board = new List<List<Rectangle>>();
            this.stackedBlockPosition = new List<int[]>();
            this.brushList = new List<List<Brush>>();
            this.myBlockBrush = Brushes.Black;
            this.score = 0;

            SetOperatingBlockPosition();

            for (int y = 0; y < MyTetrisForm.frameLimitY; y += 1)
            {
                List<Rectangle> line = new List<Rectangle>();
                List<Brush> lineOfBrushList = new List<Brush>();
                for (int x = 0; x < MyTetrisForm.frameLimitX; x++)
                {
                    line.Add(new Rectangle(x * blockSideLength, y * blockSideLength, blockSideLength, blockSideLength));
                    lineOfBrushList.Add(Brushes.Black);
                }
                this.board.Add(line);
                this.brushList.Add(lineOfBrushList);
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
                    if (this.stackedBlockPosition.Contains(point, comparer))
                    {
                        e.Graphics.FillRectangle(this.brushList[y][x], this.board[y][x]);
                        e.Graphics.DrawRectangle(blackPen, this.board[y][x]);
                    }
                    else if (IsContainesPoint(point))
                    {
                        e.Graphics.FillRectangle(myBlockBrush, this.board[y][x]);
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

        private bool IsOperatingBlockInsideFrame(int[][] operatingBlockPosition, int yMovingDistance, bool isBlockInitial = false)
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
                    if (isBlockInitial)
                    {
                        return false;
                    }

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
                int[] sepBlockPosition = originalBlockPosition[oneDimensionalIndex];
                copyBlockPosition[oneDimensionalIndex][0] = sepBlockPosition[0];
                copyBlockPosition[oneDimensionalIndex][1] = sepBlockPosition[1];
            }
        }

        private void StackOperatingBlockPosition()
        {
            for (int oneDimensionalIndex = 0; oneDimensionalIndex < this.blockPosition.GetLength(0); oneDimensionalIndex++)
            {
                int[] sepBlockPosition = this.blockPosition[oneDimensionalIndex];
                this.stackedBlockPosition.Add(sepBlockPosition);
                SetBrushesToBrushList(sepBlockPosition);
            }
            EraseStackedBlocks();
            SetOperatingBlockPosition();
        }

        private void EraseStackedBlocks()
        {
            int erasedRowNumber = 0;
            int topErasedRowNumber = 0;
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
                        erasedRowNumber += 1;
                        topErasedRowNumber = indexY;
                    }
                }
            }
            Invalidate();
            DropStackedBlocks(erasedRowNumber, topErasedRowNumber);
            this.score += (100 * erasedRowNumber);
            this.ScoreLabel.Text = this.score.ToString();
        }

        private void DropStackedBlocks(int erasedRowNumber, int topErasedRowNumber)
        {
            foreach(var sepBlockPosition in this.stackedBlockPosition)
            {
                if (sepBlockPosition[1] < topErasedRowNumber)
                {
                    int PositionX = sepBlockPosition[0];
                    int PositionY = sepBlockPosition[1];
                    Brush dropBlocksBrush = this.brushList[PositionY][PositionX];
                    sepBlockPosition[1] += erasedRowNumber;
                    this.brushList[PositionY + erasedRowNumber][PositionX] = dropBlocksBrush;
                }
            }
            Invalidate();
        }

        private void SetBrushesToBrushList(int[] sepBlockPosition)
        {
            int positionY = sepBlockPosition[1];
            int positionX = sepBlockPosition[0];
            this.brushList[positionY][positionX] = this.myBlockBrush;
        }

        private bool IsGameOver()
        {
            return !IsOperatingBlockInsideFrame(this.blockPosition, 1, true);
        }

        private void SetOperatingBlockPosition()
        {
            Random randIndex = new System.Random();
            switch (randIndex.Next(0, 7))
            {
                case 0:
                    this.blockPosition = new int[][]
                    {
                        new int[] {3, 0},
                        new int[] {3, 1},
                        new int[] {3, 2},
                        new int[] {3, 3},
                    };
                    this.myBlockBrush = Brushes.DeepSkyBlue;
                    break;
                case 1:
                    this.blockPosition = new int[][]
                    {
                        new int[] {3, 0},
                        new int[] {3, 1},
                        new int[] {3, 2},
                        new int[] {4, 1},
                    };
                    this.myBlockBrush = Brushes.DeepPink;
                    break;
                case 2:
                    this.blockPosition = new int[][]
                    {
                        new int[] {3, 0},
                        new int[] {3, 1},
                        new int[] {3, 2},
                        new int[] {4, 0},
                    };
                    this.myBlockBrush = Brushes.LightSkyBlue;
                    break;
                case 3:
                    this.blockPosition = new int[][]
                    {
                        new int[] {3, 0},
                        new int[] {3, 1},
                        new int[] {3, 2},
                        new int[] {4, 2},
                    };
                    this.myBlockBrush = Brushes.Orange;
                    break;
                case 4:
                    this.blockPosition = new int[][]
                    {
                        new int[] {3, 0},
                        new int[] {3, 1},
                        new int[] {4, 0},
                        new int[] {4, 1},
                    };
                    this.myBlockBrush = Brushes.Chocolate;
                    break;
                case 5:
                    this.blockPosition = new int[][]
                    {
                        new int[] {3, 0},
                        new int[] {4, 0},
                        new int[] {4, 1},
                        new int[] {5, 1},
                    };
                    this.myBlockBrush = Brushes.Red;
                    break;
                case 6:
                    this.blockPosition = new int[][]
                    {
                        new int[] {3, 1},
                        new int[] {4, 1},
                        new int[] {4, 0},
                        new int[] {4, 2},
                    };
                    this.myBlockBrush = Brushes.LightGreen;
                    break;
            }
            if (IsGameOver())
            {
                this.Close();
            }
        }
    }

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
}
