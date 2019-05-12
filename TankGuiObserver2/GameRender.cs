namespace TankGuiObserver2
{
    using System;
    using System.Collections.Generic;

    using SharpDX;
    using SharpDX.Direct2D1;
    using SharpDX.Mathematics.Interop;

    using TankCommon.Enum;
    using TankCommon.Objects;
    
    struct ImmutableObject
    {
        public char ColorBrushIndex;
        public RawRectangleF Rectangle;
        public ImmutableObject(
            char colorBrushIndex,
            RawRectangleF rectangle)
        {
            ColorBrushIndex = colorBrushIndex;
            Rectangle = rectangle;
        }
    }

    struct DestuctiveWalls
    {
        public char ColorBrushIndex;
        public int RowIndex;
        public int ColumnIndex;
        public RawRectangleF Rectangle;
        public DestuctiveWalls(
            char colorBrushIndex, 
            int row, int column,
            RawRectangleF rectangle)
        {
            ColorBrushIndex = colorBrushIndex;
            RowIndex = row;
            ColumnIndex = column;
            Rectangle = rectangle;
        }
    }

    class GameRender : System.IDisposable
    {
        private RenderTarget _renderTarget2D;
        private Factory _factory2D;

        private bool _isImmutableObjectsInitialized;
        private bool _isDestructiveObjectsInitialized;
        private bool _isMapSet;
        private List<ImmutableObject> _immutableMapObjects;
        private List<DestuctiveWalls> _destuctiveWallsObjects;
        private SolidColorBrush[] _mapObjectsColors;
        private Map _map;
        int _mapWidth;
        int _mapHeight;
        float _zoomWidth;
        float _zoomHeight;

        public GameRender(Factory factory2D,
            RenderTarget renderTarget)
        {
            _factory2D = factory2D;
            _renderTarget2D = renderTarget;
            
            _immutableMapObjects = new List<ImmutableObject>();
            _destuctiveWallsObjects = new List<DestuctiveWalls>();

            _mapObjectsColors = new SolidColorBrush[] {
                new SolidColorBrush(_renderTarget2D, Color.White),
                new SolidColorBrush(_renderTarget2D, Color.DarkRed),
                new SolidColorBrush(_renderTarget2D, Color.DarkBlue),
                new SolidColorBrush(_renderTarget2D, Color.GreenYellow),
                new SolidColorBrush(_renderTarget2D, Color.SandyBrown),
                new SolidColorBrush(_renderTarget2D, Color.Yellow), //bullet speed
                new SolidColorBrush(_renderTarget2D, Color.Red), //Damage
                new SolidColorBrush(_renderTarget2D, Color.Aquamarine), //Health
                new SolidColorBrush(_renderTarget2D, Color.Blue), //MaxHP
                new SolidColorBrush(_renderTarget2D, Color.CornflowerBlue),//Speed
                new SolidColorBrush(_renderTarget2D, Color.LightYellow)//Bullet
            };

        }

        [System.Runtime.CompilerServices.MethodImpl(256)]
        protected void FillBlock(RawRectangleF rectangle, SolidColorBrush brush)
        {
            _renderTarget2D.FillRectangle(rectangle, brush);
        }

        public void DrawMap(Map map)
        {
            _map = map;
            // рисуем всю карту
            if (!_isMapSet)
            {
                _isMapSet = true;
                _mapWidth = map.MapWidth;
                _mapHeight = map.MapHeight;

                _zoomWidth = _renderTarget2D.Size.Width / _mapWidth;
                _zoomHeight = _renderTarget2D.Size.Height / _mapHeight;
            }

            //неизменяемые
            RawRectangleF rawRectangleTemp = new RawRectangleF();
            if (!_isImmutableObjectsInitialized)
            {
                _isImmutableObjectsInitialized = true;

                int i, j;

                rawRectangleTemp.Left = 0;
                rawRectangleTemp.Top = 0;
                rawRectangleTemp.Right = (_mapWidth - 1) * _zoomWidth + _zoomWidth;
                rawRectangleTemp.Bottom = 4 * _zoomHeight + _zoomHeight;
                _immutableMapObjects.Add(new ImmutableObject((char)0, rawRectangleTemp));
                FillBlock(rawRectangleTemp, _mapObjectsColors[0]);

                rawRectangleTemp.Left = 0;
                rawRectangleTemp.Top = (_mapHeight - 6) * _zoomHeight;
                rawRectangleTemp.Right = (_mapWidth - 1) * _zoomWidth + _zoomWidth;
                rawRectangleTemp.Bottom = (_mapHeight - 1) * _zoomHeight + _zoomHeight;
                _immutableMapObjects.Add(new ImmutableObject((char)0, rawRectangleTemp));
                FillBlock(rawRectangleTemp, _mapObjectsColors[0]);

                rawRectangleTemp.Left = 0;
                rawRectangleTemp.Top = 5 * _zoomHeight;
                rawRectangleTemp.Right = 4 * _zoomWidth + _zoomWidth;
                rawRectangleTemp.Bottom = (_mapHeight - 5) * _zoomHeight + _zoomHeight;
                _immutableMapObjects.Add(new ImmutableObject((char)0, rawRectangleTemp));
                FillBlock(rawRectangleTemp, _mapObjectsColors[0]);

                rawRectangleTemp.Left = (_mapWidth - 6) * _zoomWidth + _zoomWidth;
                rawRectangleTemp.Top = 5 * _zoomHeight;
                rawRectangleTemp.Right = (_mapWidth - 1) * _zoomWidth + _zoomWidth;
                rawRectangleTemp.Bottom = (_mapHeight - 5) * _zoomHeight + _zoomHeight;
                _immutableMapObjects.Add(new ImmutableObject((char)0, rawRectangleTemp));
                FillBlock(rawRectangleTemp, _mapObjectsColors[0]);
                
                for (i = 5; i < (_mapHeight-5); i++)
                {
                    for (j = 5; j < (_mapWidth-5); j++)
                    {
                        CellMapType cellMapType = map[i, j];
                        
                        if (cellMapType == CellMapType.Wall)
                        {
                            rawRectangleTemp.Left = j * _zoomWidth;
                            rawRectangleTemp.Top = i * _zoomHeight;
                            rawRectangleTemp.Right = j * _zoomWidth + _zoomWidth;
                            rawRectangleTemp.Bottom = i * _zoomHeight + _zoomHeight;
                            _immutableMapObjects.Add(new ImmutableObject((char)0, rawRectangleTemp));
                            FillBlock(rawRectangleTemp, _mapObjectsColors[0]);
                        }
                        else if (cellMapType == CellMapType.Water)
                        {
                            rawRectangleTemp.Left = j * _zoomWidth;
                            rawRectangleTemp.Top = i * _zoomHeight;
                            rawRectangleTemp.Right = j * _zoomWidth + _zoomWidth;
                            rawRectangleTemp.Bottom = i * _zoomHeight + _zoomHeight;
                            _immutableMapObjects.Add(new ImmutableObject((char)2, rawRectangleTemp));
                            FillBlock(rawRectangleTemp, _mapObjectsColors[2]);
                        }
                        else if (cellMapType == CellMapType.Grass)
                        {
                            rawRectangleTemp.Left = j * _zoomWidth;
                            rawRectangleTemp.Top = i * _zoomHeight;
                            rawRectangleTemp.Right = j * _zoomWidth + _zoomWidth;
                            rawRectangleTemp.Bottom = i * _zoomHeight + _zoomHeight;
                            _immutableMapObjects.Add(new ImmutableObject((char)3, rawRectangleTemp));
                            FillBlock(rawRectangleTemp, _mapObjectsColors[3]);
                        }
                    }
                }
            }
            else
            {
                foreach (var obj in _immutableMapObjects)
                {
                    FillBlock(obj.Rectangle, _mapObjectsColors[obj.ColorBrushIndex]);
                }
            }

            if (!_isDestructiveObjectsInitialized)
            {
                _isDestructiveObjectsInitialized = true;
                for (var i = 5; i < (_mapHeight - 5); i++)
                {
                    for (var j = 5; j < (_mapWidth - 5); j++)
                    {
                        var c = map[i, j];
                        if (c == CellMapType.DestructiveWall)
                        {
                            rawRectangleTemp.Left = j * _zoomWidth;
                            rawRectangleTemp.Top = i * _zoomHeight;
                            rawRectangleTemp.Right = j * _zoomWidth + _zoomWidth;
                            rawRectangleTemp.Bottom = i * _zoomHeight + _zoomHeight;
                            _destuctiveWallsObjects.Add(new DestuctiveWalls((char)1, i, j, rawRectangleTemp));
                            FillBlock(rawRectangleTemp, _mapObjectsColors[1]);
                        }
                    }
                }
            }
            else
            {
                foreach (var obj in _destuctiveWallsObjects)
                {
                    if (map[obj.RowIndex, obj.ColumnIndex] == CellMapType.DestructiveWall)
                    {
                        FillBlock(obj.Rectangle, _mapObjectsColors[obj.ColorBrushIndex]);
                    }
                }
            }
        }

        public void DrawInteractiveObjects(List<BaseInteractObject> baseInteractObjects)
        {
            RawRectangleF rawRectangleTemp = new RawRectangleF();
            foreach (var obj in baseInteractObjects)
            {
                if (obj is TankObject tankObject)
                {
                    rawRectangleTemp.Left = Convert.ToSingle(tankObject.Rectangle.LeftCorner.Left) * _zoomWidth;
                    rawRectangleTemp.Top = Convert.ToSingle(tankObject.Rectangle.LeftCorner.Top) * _zoomHeight;
                    rawRectangleTemp.Right = Convert.ToSingle(tankObject.Rectangle.LeftCorner.Left + tankObject.Rectangle.Width) * _zoomWidth;
                    rawRectangleTemp.Bottom = Convert.ToSingle(tankObject.Rectangle.LeftCorner.Top + tankObject.Rectangle.Height) * _zoomHeight;
                    FillBlock(rawRectangleTemp, _mapObjectsColors[4]);
                }
                else if (obj is UpgradeInteractObject upgradeObject)
                {
                    switch (upgradeObject.Type)
                    {
                        case UpgradeType.BulletSpeed:
                        {
                            rawRectangleTemp.Left = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left) * _zoomWidth;
                            rawRectangleTemp.Top = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top) * _zoomHeight;
                            rawRectangleTemp.Right = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left + upgradeObject.Rectangle.Width) * _zoomWidth;
                            rawRectangleTemp.Bottom = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top + upgradeObject.Rectangle.Height) * _zoomHeight;
                            FillBlock(rawRectangleTemp, _mapObjectsColors[5]);
                        } break;
                        case UpgradeType.Damage:
                        {
                            rawRectangleTemp.Left = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left) * _zoomWidth;
                            rawRectangleTemp.Top = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top) * _zoomHeight;
                            rawRectangleTemp.Right = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left + upgradeObject.Rectangle.Width) * _zoomWidth;
                            rawRectangleTemp.Bottom = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top + upgradeObject.Rectangle.Height) * _zoomHeight;
                            FillBlock(rawRectangleTemp, _mapObjectsColors[6]);
                        } break;
                        case UpgradeType.Health:
                        {
                            rawRectangleTemp.Left = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left) * _zoomWidth;
                            rawRectangleTemp.Top = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top) * _zoomHeight;
                            rawRectangleTemp.Right = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left + upgradeObject.Rectangle.Width) * _zoomWidth;
                            rawRectangleTemp.Bottom = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top + upgradeObject.Rectangle.Height) * _zoomHeight;
                            FillBlock(rawRectangleTemp, _mapObjectsColors[7]);
                        } break;
                        case UpgradeType.MaxHp:
                        {
                            rawRectangleTemp.Left = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left) * _zoomWidth;
                            rawRectangleTemp.Top = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top) * _zoomHeight;
                            rawRectangleTemp.Right = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left + upgradeObject.Rectangle.Width) * _zoomWidth;
                            rawRectangleTemp.Bottom = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top + upgradeObject.Rectangle.Height) * _zoomHeight;
                            FillBlock(rawRectangleTemp, _mapObjectsColors[8]);
                        } break;
                        case UpgradeType.Speed:
                        {
                            rawRectangleTemp.Left = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left) * _zoomWidth;
                            rawRectangleTemp.Top = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top) * _zoomHeight;
                            rawRectangleTemp.Right = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Left + upgradeObject.Rectangle.Width) * _zoomWidth;
                            rawRectangleTemp.Bottom = Convert.ToSingle(upgradeObject.Rectangle.LeftCorner.Top + upgradeObject.Rectangle.Height) * _zoomHeight;
                            FillBlock(rawRectangleTemp, _mapObjectsColors[9]);
                        } break;
                    }
                }
                else if (obj is BulletObject bulletObject)
                {
                    rawRectangleTemp.Left = Convert.ToSingle(bulletObject.Rectangle.LeftCorner.Left) * _zoomWidth;
                    rawRectangleTemp.Top = Convert.ToSingle(bulletObject.Rectangle.LeftCorner.Top) * _zoomHeight;
                    rawRectangleTemp.Right = Convert.ToSingle(bulletObject.Rectangle.LeftCorner.Left + bulletObject.Rectangle.Width) * _zoomWidth;
                    rawRectangleTemp.Bottom = Convert.ToSingle(bulletObject.Rectangle.LeftCorner.Top + bulletObject.Rectangle.Height) * _zoomHeight;
                    FillBlock(rawRectangleTemp, _mapObjectsColors[10]);
                }
            }
        }
        
        public void Dispose()
        {
            for (int mapIndex = 0; mapIndex < _mapObjectsColors.Length; mapIndex++)
            {
                _mapObjectsColors[mapIndex].Dispose();
            }
        }

    }
}
