﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SFML.Audio;


namespace TripOverTime.EngineNamespace
{
    public class Engine
    {
        bool CLOSE = false;
        Stopwatch _timer;
        Checkpoint _checkpoint;
        List<Position2> _verifHeal2;

        SFML.Graphics.RenderWindow _window;
        Menu _menu;
        Game _game; // Contient Map, Player, Monster
        Game _game2;
        Settings _settings;
        GUI _gui;
        Font _globalFont;
        string _oldOrientation;
        Music _musicJeux;
        Music _musicMenu;


        public Engine(SFML.Graphics.RenderWindow window)
        {
            _window = window;
            _menu = new Menu(window);
            if (_musicMenu == null)
            {
                _musicMenu = new Music("..\\..\\..\\..\\Music\\Daft_Punk_-_Veridis_Quo_Cyberdesign_Remix.ogg");
                _musicMenu.Play();
            }
            _settings = new Settings(this, window);
            _gui = new GUI(this, window);
            _globalFont = new Font(@"..\..\..\..\Assets\Fonts\Blanka-Regular.ttf");
            _timer = new Stopwatch();
            _timer.Start();
            _verifHeal2 = new List<Position2>();
            _oldOrientation = "null";
        }

        public void StartGame(string mapPath, bool multiplayer = false)
        {
            // Ecran chargement
            _gui.ShowLoading(1);

            //Verify if it's a map file
            if (!mapPath.EndsWith(".totmap")) throw new ArgumentException("The map file is not correct (.totmap)");
            // Open map file
            string text = File.ReadAllText(mapPath);
            if (String.IsNullOrEmpty(text)) throw new FileLoadException("File is empty ?");

            _gui.ShowLoading(5);

            // Get player
            // path x y life atk
            string[] strPlayer = StringBetweenString(text, "PLAYER", "PLAYEREND").Split(" ");

            _game = new Game(this, mapPath, strPlayer[0], new Position(Convert.ToSingle(strPlayer[1]), Convert.ToSingle(strPlayer[2])), new Position2(Convert.ToSingle(strPlayer[1]), Convert.ToSingle(strPlayer[2])), Convert.ToUInt16(strPlayer[3]), Convert.ToUInt16(strPlayer[4]), multiplayer); //0, 3

            _gui.ShowLoading(90);

            //Music
            _musicMenu.Stop();
            if (text.Contains("MUSICEND"))
            {
                string[] strMusic = StringBetweenString(text, "MUSIC", "MUSICEND").Split(" ");
                if (!String.IsNullOrEmpty(strMusic[0]))
                {
                    _musicJeux = new Music(strMusic[0]);
                    _musicJeux.Play();
                }
            }
            
        }
        public void StartGame2(string mapPath)
        {
            //Verify if it's a map file
            if (!mapPath.EndsWith(".totmap")) throw new ArgumentException("The map file is not correct (.totmap)");
            // Open map file
            string text = File.ReadAllText(mapPath);
            if (String.IsNullOrEmpty(text)) throw new FileLoadException("File is empty ?");

            // Get player
            // path x y life atk
            string[] strPlayer = StringBetweenString(text, "PLAYER", "PLAYEREND").Split(" ");

            _game = new Game(this, mapPath, strPlayer[0], new Position(Convert.ToSingle(strPlayer[1]), Convert.ToSingle(strPlayer[2])), new Position2(Convert.ToSingle(strPlayer[1]), Convert.ToSingle(strPlayer[2])), Convert.ToUInt16(strPlayer[3]), Convert.ToUInt16(strPlayer[4]), false); //0, 3
            _game2 = new Game(this, mapPath, strPlayer[0], new Position(Convert.ToSingle(strPlayer[1]), Convert.ToSingle(strPlayer[2])), new Position2(Convert.ToSingle(strPlayer[1]), Convert.ToSingle(strPlayer[2])), Convert.ToUInt16(strPlayer[3]), Convert.ToUInt16(strPlayer[4]), true); //0, 3
                                                                                                                                                                                                                                                                                               //Music
            string[] strMusic = StringBetweenString(text, "MUSIC", "MUSICEND").Split(" ");
            _musicMenu.Stop();
            if (!String.IsNullOrEmpty(strMusic[0]))
            {
                _musicJeux = new Music(strMusic[0]);
                _musicJeux.Play();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if player is alive</returns>
        public short GameTick()
        {
            if (_game == null) throw new Exception("Game not started!");

            //Events
            _gui.Events();

            //Gravity 4 player
            Sprite sToPositive = null;
            Sprite sToNegative = null;

            //Console.WriteLine("Player: " + _game.GetPlayer.RealPosition.X + ";" + _game.GetPlayer.RealPosition.Y);
            // Block sous player
            if (_game.GetPlayer.Orientation == "right")
            {
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetPlayer.RealPosition.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(_game.GetPlayer.RealPosition.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetPlayer.RealPosition.X + 0.3f, MidpointRounding.ToNegativeInfinity), (float)Math.Round(_game.GetPlayer.RealPosition.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
            }
            else
            {
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetPlayer.RealPosition.X - 0.5f, MidpointRounding.ToPositiveInfinity), (float)Math.Round(_game.GetPlayer.RealPosition.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetPlayer.RealPosition.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(_game.GetPlayer.RealPosition.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
            }

            //Console.WriteLine("Get+ " + sToPositive.Name + " at " + (float)Math.Round(_game.GetPlayer.RealPosition.X, MidpointRounding.ToPositiveInfinity) + ";" + (float)Math.Round(_game.GetPlayer.RealPosition.Y - 1, MidpointRounding.ToPositiveInfinity));
            //Console.WriteLine("Get- " + sToNegative.Name + " at " + (float)Math.Round(_game.GetPlayer.RealPosition.X, MidpointRounding.ToNegativeInfinity) + ";" + (float)Math.Round(_game.GetPlayer.RealPosition.Y - 1, MidpointRounding.ToPositiveInfinity));

            if ((sToPositive != null && !sToPositive.IsSolid && sToNegative != null && !sToNegative.IsSolid))
            {
                //Block under player isn't solid
                if (sToPositive.IsDangerous || sToNegative.IsDangerous)
                {
                    //DIE
                    _game.GetPlayer.KilledBy = "Trap";
                    MusicStop(_musicJeux);
                    return -1;
                }
                _game.GetPlayer.Gravity();
                _oldOrientation = _game.GetPlayer.Orientation;
            }
            else
            {
                _game.GetPlayer.IsJumping = false;
                _game.GetPlayer.RoundY(); // Don't stuck player in ground
            }

            // Verify if in a block when orientation change after gravity
            if (_oldOrientation != "null")
            {
                Sprite sTemp = null;

                if (_game.GetPlayer.Orientation == "left" && _oldOrientation == "right")
                {
                    _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetPlayer.RealPosition.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(_game.GetPlayer.RealPosition.Y, MidpointRounding.ToPositiveInfinity)), out sTemp);
                    if (sTemp != null && sTemp.IsSolid) //In a block
                    {
                        _game.GetPlayer.RealPosition.X += 0.3f;
                    }
                    _oldOrientation = "null";
                }
                else if (_game.GetPlayer.Orientation == "right" && _oldOrientation == "left")
                {
                    _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetPlayer.RealPosition.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(_game.GetPlayer.RealPosition.Y, MidpointRounding.ToPositiveInfinity)), out sTemp);
                    if (sTemp != null && sTemp.IsSolid) //In a block
                    {
                        _game.GetPlayer.RealPosition.X -= 0.5f;
                    }
                    _oldOrientation = "null";
                }
            }

            //Heal
            List<Position> heart = _game.GetMapObject.GetHeart;
            foreach (Position position in heart)
            {
                if (_game.GetPlayer.RealPosition.X == position.X && _game.GetMapObject.GetMap[position].Id != "A")
                {
                    _game.GetPlayer.GetLife.BonusPoint(1);
                    _game.GetMapObject.GetMap[position] = _game.GetMapObject.GetSpriteChange;
                    _gui.LoadMap();
                }
            }

            //strength
            List<Position> star = _game.GetMapObject.GetStar;
            foreach (Position position in star)
            {
                if (_game.GetPlayer.RealPosition.X == position.X && _game.GetMapObject.GetMap[position].Id != "A")
                {

                    _game.GetPlayer.GetAttack++;
                    _game.GetMapObject.GetMap[position] = _game.GetMapObject.GetSpriteChange;
                    _gui.LoadMap();

                }
            }

            if (!_game.GetPlayer.IsAlive)
            {
                //DIE
                _game.GetPlayer.KilledBy = "Monster";
                MusicStop(_musicJeux);
                return -1;
            }

            //Monsters move + Attack
            foreach (Monster m in _game.GetMonsters)
            {
                if (m.Position.X > _game.GetPlayer.RealPosition.X && m.isAlive) //left
                {
                    m.Orientation = "left";
                }
                else if (m.Position.X < _game.GetPlayer.RealPosition.X && m.isAlive) //right
                {
                    m.Orientation = "right";
                }

                if (!m.isAlive)
                {
                    m.MonsterDead();
                }
                else if (m.Position.X - 4 < _game.GetPlayer.RealPosition.X && m.Position.X - 1.2 > _game.GetPlayer.RealPosition.X || m.Position.X + 4 > _game.GetPlayer.RealPosition.X && m.Position.X + 1.2 < _game.GetPlayer.RealPosition.X)
                {
                    m.MonsterMove();
                }

                if (m.Position.X + m.Range > _game.GetPlayer.RealPosition.X && m.Position.X - m.Range < _game.GetPlayer.RealPosition.X && m.isAlive) //attack
                {
                    m.MonsterAttack();
                }
            }

            //Gravity 4 monsters
            foreach (Monster m in _game.GetMonsters)
            {
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(m.Position.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(m.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(m.Position.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(m.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);

                if (m.Orientation == "right")
                {
                    _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(m.Position.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(m.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
                    _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(m.Position.X + 0.3f, MidpointRounding.ToNegativeInfinity), (float)Math.Round(m.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
                }
                else
                {
                    _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(m.Position.X - 0.5f, MidpointRounding.ToPositiveInfinity), (float)Math.Round(m.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
                    _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(m.Position.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(m.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
                }

                if (sToPositive != null && !sToPositive.IsSolid && sToNegative != null && !sToNegative.IsSolid)
                {
                    //Block under monster isn't solid
                    if (sToPositive.IsDangerous || sToNegative.IsDangerous)
                    {
                        //DIE
                        m.life.CurrentPoint = 0;
                        m.MonsterDead();
                    }
                    //Block under monster isn't solid
                    m.Gravity();
                }
                else
                {
                    m.IsMoving = false;
                    m.RoundY(); // Don't stuck monster in ground
                }
            }


            // boss
            if (_game.GetBoss != null)
            {
                if (!_game.GetBoss.IsAlive)
                {
                    _game.GetBoss.BossDead();
                }
                else
                {
                    if (_game.GetBoss.Position.X > _game.GetPlayer.RealPosition.X) //left
                    {
                        _game.GetBoss.Orientation = "left";
                    }
                    else if (_game.GetBoss.Position.X < _game.GetPlayer.RealPosition.X) //right
                    {
                        _game.GetBoss.Orientation = "right";
                    }

                    _game.GetBoss.GetBossSprite.BossOrientation(_game.GetBoss);

                    if (_game.GetBoss.Position.X + 3 > _game.GetPlayer.RealPosition.X && _game.GetBoss.Position.X - 3 < _game.GetPlayer.RealPosition.X) //attack
                    {
                        if (_game.GetBoss.Position.X - 1 > _game.GetPlayer.RealPosition.X || _game.GetBoss.Position.X + 1 < _game.GetPlayer.RealPosition.X)
                        {
                            _game.GetBoss.BossMove();
                        }
                        _game.GetBoss.BossAttack();
                    }

                    else if (_game.GetBoss.Position.X - 6 < _game.GetPlayer.RealPosition.X && _game.GetBoss.Position.X - 1 > _game.GetPlayer.RealPosition.X || _game.GetBoss.Position.X + 6 > _game.GetPlayer.RealPosition.X && _game.GetBoss.Position.X + 1 < _game.GetPlayer.RealPosition.X)
                    {
                        _game.GetBoss.BossMove();
                        _game.GetBoss.GetBossSprite.BossMoveAnimation(_game.GetBoss);
                    }
                }

                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetBoss.Position.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(_game.GetBoss.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetBoss.Position.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(_game.GetBoss.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
                if (sToPositive != null && !sToPositive.IsSolid && sToNegative != null && !sToNegative.IsSolid)
                {
                    //Block under monster isn't solid
                    _game.GetBoss.Gravity();
                }
                else
                {
                    _game.GetBoss.IsMoving = false;
                    _game.GetBoss.RoundY(); // Don't stuck monster in ground
                }

            }

            // Recalibrate float
            _game.GetPlayer.RoundX();

            // WIN !!!
            Position end = _game.GetMapObject.GetEndPosition;
            if (end.X <= _game.GetPlayer.RealPosition.X)
            {
                Console.WriteLine("YOUWINNNNNNNNNN");
                MusicStop(_musicJeux);
                // SHOW WIN MENU !
                return 0;
            }

            // Dead return -1;

            return 1;
        }

        public short GameTick2()
        {
            if (_game == null) throw new Exception("Game not started!");
            if (_game2 == null) throw new Exception("Game 2 not started!");

            //Events
            _gui.Events2();


            //Console.Write("Player : " + _game.GetPlayer.RealPosition.X + ";" + _game.GetPlayer.RealPosition.Y);
            //Console.WriteLine(" | Monster1 : " + _game.GetMonsters[0].Position.X + ";" + _game.GetMonsters[0].Position.Y + " " + _game.GetMonsters[0].life.GetCurrentPoint() + "HP.");
            //Gravity 4 player
            Sprite sToPositive = null;
            Sprite sToNegative = null;
            Sprite sToPositive2 = null;
            Sprite sToNegative2 = null;
            _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetPlayer.RealPosition.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(_game.GetPlayer.RealPosition.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
            _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetPlayer.RealPosition.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(_game.GetPlayer.RealPosition.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
            if (sToPositive != null && !sToPositive.IsSolid && sToNegative != null && !sToNegative.IsSolid)
            {
                //Block under player isn't solid
                if (sToPositive.IsDangerous || sToNegative.IsDangerous)
                {
                    //DIE
                    _game.GetPlayer.KilledBy = "Trap";
                    return -1;
                }
                _game.GetPlayer.Gravity();
            }
            else
            {
                _game.GetPlayer.IsJumping = false;
                _game.GetPlayer.RoundY(); // Don't stuck player in ground
            }
            //Heal
            List<Position> heart = _game.GetMapObject.GetHeart;
            foreach (Position position in heart)
            {
                if (_game.GetPlayer.RealPosition.X == position.X && _game.GetMapObject.GetMap[position].Id != "A")
                {
                    _game.GetPlayer.GetLife.BonusPoint(1);
                    _game.GetMapObject.GetMap[position] = _game.GetMapObject.GetSpriteChange;
                    _gui.LoadMap();
                }
            }


            //strength
            List<Position> star = _game.GetMapObject.GetStar;
            foreach (Position position in star)
            {
                if (_game.GetPlayer.RealPosition.X == position.X && _game.GetMapObject.GetMap[position].Id != "A")
                {

                    _game.GetPlayer.GetAttack++;
                    _game.GetMapObject.GetMap[position] = _game.GetMapObject.GetSpriteChange;
                    _gui.LoadMap();

                }
            }



            if (!_game.GetPlayer.IsAlive)
            {
                //DIE
                _game.GetPlayer.KilledBy = "Monster";
                return -1;
            }


            //Monsters move + Attack
            foreach (Monster m in _game.GetMonsters)
            {


                if (!m.isAlive)
                {
                    m.MonsterDead();
                }
                else
                {
                    if (m.Position.X > _game.GetPlayer.RealPosition.X) //left
                    {
                        m.Orientation = "left";
                    }
                    else if (m.Position.X < _game.GetPlayer.RealPosition.X) //right
                    {
                        m.Orientation = "right";
                    }

                    if (m.Position.X - 4 < _game.GetPlayer.RealPosition.X && m.Position.X - 1 > _game.GetPlayer.RealPosition.X || m.Position.X + 4 > _game.GetPlayer.RealPosition.X && m.Position.X + 1 < _game.GetPlayer.RealPosition.X)
                    {
                        m.MonsterMove();
                    }

                    {
                        m.MonsterAttack();
                    }
                }
            }

            //Gravity 4 monsters
            foreach (Monster m in _game.GetMonsters)
            {
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(m.Position.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(m.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
                _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(m.Position.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(m.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
                if (sToPositive != null && !sToPositive.IsSolid && sToNegative != null && !sToNegative.IsSolid)
                {
                    
                    //Block under monster isn't solid
                    if (sToPositive.IsDangerous || sToNegative.IsDangerous)
                    {
                        //DIE
                        m.life.CurrentPoint = 0;
                        m.MonsterDead();
                    }
                    m.Gravity();
                }
                else
                {
                    m.IsMoving = false;
                    m.RoundY(); // Don't stuck monster in ground
                }
            }

            // boss
            if (!_game.GetBoss.IsAlive)
            {
                _game.GetBoss.BossDead();
            }
            else
            {
                if (_game.GetBoss.Position.X > _game.GetPlayer.RealPosition.X) //left
                {
                    _game.GetBoss.Orientation = "left";
                }
                else if (_game.GetBoss.Position.X < _game.GetPlayer.RealPosition.X) //right
                {
                    _game.GetBoss.Orientation = "right";
                }

                _game.GetBoss.GetBossSprite.BossOrientation(_game.GetBoss);

                if (_game.GetBoss.Position.X + 3 > _game.GetPlayer.RealPosition.X && _game.GetBoss.Position.X - 3 < _game.GetPlayer.RealPosition.X) //attack
                {
                    if (_game.GetBoss.Position.X - 1 > _game.GetPlayer.RealPosition.X || _game.GetBoss.Position.X + 1 < _game.GetPlayer.RealPosition.X)
                    {
                        _game.GetBoss.BossMove();
                    }
                    _game.GetBoss.BossAttack();
                }

                else if (_game.GetBoss.Position.X - 6 < _game.GetPlayer.RealPosition.X && _game.GetBoss.Position.X - 1 > _game.GetPlayer.RealPosition.X || _game.GetBoss.Position.X + 6 > _game.GetPlayer.RealPosition.X && _game.GetBoss.Position.X + 1 < _game.GetPlayer.RealPosition.X)
                {
                    _game.GetBoss.BossMove();
                    _game.GetBoss.GetBossSprite.BossMoveAnimation(_game.GetBoss);
                }
            }

            _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetBoss.Position.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(_game.GetBoss.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
            _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetBoss.Position.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(_game.GetBoss.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
            if (sToPositive != null && !sToPositive.IsSolid && sToNegative != null && !sToNegative.IsSolid)
            {
                //Block under monster isn't solid
                _game.GetBoss.Gravity();
            }
            else
            {
                _game.GetBoss.IsMoving = false;
                _game.GetBoss.RoundY(); // Don't stuck monster in ground
            }


            // Recalibrate float
            _game.GetPlayer.RoundX();
            // WIN !!!
            Position end = _game.GetMapObject.GetEndPosition;
            if (end.X <= _game.GetPlayer.RealPosition.X)
            {
                Console.WriteLine("YOUWINNNNNNNNNN");
                // SHOW WIN MENU !
                return 0;
            }
            //win player 2
            if (end.X <= _game2.GetPlayer2.RealPosition2.X2)
            {
                Console.WriteLine("YOUWINNNNNNNNNN");
                // SHOW WIN MENU !
                return 2;
            }

            //-------------------------------------------------------player 2
            _game2.GetMapObject2.GetMap2.TryGetValue(new Position2((float)Math.Round(_game2.GetPlayer2.RealPosition2.X2, MidpointRounding.ToPositiveInfinity), (float)Math.Round(_game2.GetPlayer2.RealPosition2.Y2 - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive2);
            _game2.GetMapObject2.GetMap2.TryGetValue(new Position2((float)Math.Round(_game2.GetPlayer2.RealPosition2.X2, MidpointRounding.ToNegativeInfinity), (float)Math.Round(_game2.GetPlayer2.RealPosition2.Y2 - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative2);


            if (sToPositive2 != null && !sToPositive2.IsSolid2 && sToNegative2 != null && !sToNegative2.IsSolid2)
            {
                //Block under player isn't solid
                if (sToPositive2.IsDangerous2 || sToNegative2.IsDangerous2)
                {
                    //DIE
                    _game2.GetPlayer2.KilledBy2 = "Trap";
                    return -1;
                }
                _game2.GetPlayer2.Gravity2();
            }
            else
            {
                _game2.GetPlayer2.IsJumping2 = false;
                _game2.GetPlayer2.RoundY2(); // Don't stuck player in ground
            }
            //Heal
            List<Position2> heart2 = _game2.GetMapObject2.GetHeart2;
            foreach (Position2 position2 in heart2)
            {

                if (_game2.GetPlayer2.RealPosition2.X2 == position2.X2 && _game2.GetMapObject2.GetMap2[position2].Id2 != "A")
                {
                    
                    _game2.GetPlayer2.GetLife2.BonusPoint2(1);
                    _game2.GetMapObject2.GetMap2[position2] = _game2.GetMapObject2.GetSpriteChange2;
                    _gui.LoadMap2();
                }
            }


            //strength
            List<Position2> star2 = _game2.GetMapObject2.GetStar2;
            foreach (Position2 position2 in star2)
            {
                if (_game2.GetPlayer2.RealPosition2.X2 == position2.X2 && _game2.GetMapObject2.GetMap2[position2].Id2 != "A")
                {

                    _game2.GetPlayer2.GetAttack2++;
                    _game2.GetMapObject2.GetMap2[position2] = _game2.GetMapObject2.GetSpriteChange2;
                    _gui.LoadMap2();

                }
            }

            if (!_game2.GetPlayer2.IsAlive2)
            {
                //DIE
                _game2.GetPlayer2.KilledBy2 = "Monster";
                return -1;
            }

            //Monsters move + Attack
            foreach (Monster m2 in _game2.GetMonsters2)
            {
                if (m2.Position2.X2 > _game2.GetPlayer2.RealPosition2.X2 && m2.isAlive2) //left
                {
                    m2.Orientation2 = "left";
                }
                else if (m2.Position2.X2 < _game2.GetPlayer2.RealPosition2.X2 && m2.isAlive2) //right
                {
                    m2.Orientation2 = "right";
                }

                if (!m2.isAlive2)
                {
                    m2.MonsterDead2();
                }
                else if (m2.Position2.X2 - 4 < _game2.GetPlayer2.RealPosition2.X2 && m2.Position2.X2 - 1 > _game2.GetPlayer2.RealPosition2.X2 || m2.Position2.X2 + 4 > _game2.GetPlayer2.RealPosition2.X2 && m2.Position2.X2 + 1 < _game2.GetPlayer2.RealPosition2.X2)
                {
                    m2.MonsterMove2();
                }

                if (m2.Position2.X2 + 2 > _game2.GetPlayer2.RealPosition2.X2 && m2.Position2.X2 - 2 < _game2.GetPlayer2.RealPosition2.X2 && m2.isAlive2) //attack
                {
                    m2.MonsterAttack2();
                }
            }

            //Gravity 4 monsters
            foreach (Monster m2 in _game2.GetMonsters2)
            {
                _game2.GetMapObject2.GetMap2.TryGetValue(new Position2((float)Math.Round(m2.Position2.X2, MidpointRounding.ToPositiveInfinity), (float)Math.Round(m2.Position2.Y2 - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive2);
                _game2.GetMapObject2.GetMap2.TryGetValue(new Position2((float)Math.Round(m2.Position2.X2, MidpointRounding.ToNegativeInfinity), (float)Math.Round(m2.Position2.Y2 - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative2);
                if (sToPositive2 != null && !sToPositive2.IsSolid2 && sToNegative2 != null && !sToNegative2.IsSolid2)
                {
                    //Block under monster isn't solid
                    if (sToPositive2.IsDangerous2 || sToNegative2.IsDangerous2)
                    {
                        //DIE
                        m2.life2.CurrentPoint2 = 0;
                        m2.MonsterDead2();
                    }
                    m2.Gravity2();
                }
                else
                {
                    m2.IsMoving2 = false;
                    m2.RoundY2(); // Don't stuck monster in ground
                }



            }

            // boss
            if (!_game2.GetBoss2.IsAlive2)
            {
                _game2.GetBoss2.BossDead2();
            }
            else
            {
                if (_game2.GetBoss2.Position2.X2 > _game2.GetPlayer2.RealPosition2.X2) //left
                {
                    _game2.GetBoss2.Orientation2 = "left";
                }
                else if (_game2.GetBoss2.Position2.X2 < _game2.GetPlayer2.RealPosition2.X2) //right
                {
                    _game2.GetBoss2.Orientation2 = "right";
                }

                _game2.GetBoss2.GetBossSprite2.BossOrientation2(_game2.GetBoss2);

                if (_game2.GetBoss2.Position2.X2 + 3 > _game2.GetPlayer2.RealPosition2.X2 && _game2.GetBoss2.Position2.X2 - 3 < _game2.GetPlayer2.RealPosition2.X2) //attack
                {
                    if (_game2.GetBoss2.Position2.X2 - 1 > _game2.GetPlayer2.RealPosition2.X2 || _game2.GetBoss2.Position2.X2 + 1 < _game2.GetPlayer2.RealPosition2.X2)
                    {
                        _game2.GetBoss2.BossMove2();
                    }
                    _game2.GetBoss2.BossAttack2();
                }

                else if (_game2.GetBoss2.Position2.X2 - 6 < _game2.GetPlayer2.RealPosition2.X2 && _game2.GetBoss2.Position2.X2 - 1 > _game2.GetPlayer2.RealPosition2.X2 || _game2.GetBoss2.Position2.X2 + 6 > _game2.GetPlayer2.RealPosition2.X2 && _game2.GetBoss2.Position2.X2 + 1 < _game2.GetPlayer2.RealPosition2.X2)
                {
                    _game2.GetBoss2.BossMove2();
                    _game2.GetBoss2.GetBossSprite2.BossMoveAnimation2(_game2.GetBoss2);
                }
            }

            _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetBoss.Position.X, MidpointRounding.ToPositiveInfinity), (float)Math.Round(_game.GetBoss.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToPositive);
            _game.GetMapObject.GetMap.TryGetValue(new Position((float)Math.Round(_game.GetBoss.Position.X, MidpointRounding.ToNegativeInfinity), (float)Math.Round(_game.GetBoss.Position.Y - 1, MidpointRounding.ToPositiveInfinity)), out sToNegative);
            if (sToPositive != null && !sToPositive.IsSolid && sToNegative != null && !sToNegative.IsSolid)
            {
                //Block under monster isn't solid
                _game.GetBoss.Gravity();
            }
            else
            {
                _game.GetBoss.IsMoving = false;
                _game.GetBoss.RoundY(); // Don't stuck monster in ground
            }

            // Recalibrate float
            _game2.GetPlayer2.RoundX2();
            // WIN !!!
            Position2 end2 = _game2.GetMapObject2.GetEndPosition2;
            if (end2.X2 <= _game2.GetPlayer2.RealPosition2.X2)
            {
                Console.WriteLine("YOUWINNNNNNNNNN");
                // SHOW WIN MENU !
                return 0;
            }


            // Dead return -1;

            return 1;
        }

        public void WinMenu()
        {
            View view1 = new View(new Vector2f(Settings.XResolution / 2, Settings.YResolution / 2), new Vector2f(Settings.XResolution, Settings.YResolution));
            view1.Viewport = new FloatRect(0f, 0f, 1f, 1f);
            view1.Size = new Vector2f(Settings.XResolution, Settings.YResolution);
            _window.SetView(view1);

            SFML.Graphics.Sprite background = new SFML.Graphics.Sprite(new Texture(@"..\..\..\..\Assets\Backgrounds\time-travel-background.png"));
            if (background == null) throw new Exception("Sprite null!");

            _window.Draw(background);

            List<Text> lines = new List<Text>();
            //Lines
            lines.Add(new Text("PLAYER 1 WIN !", _globalFont, 64));
            lines.Add(new Text("in : " + _game.TimeElapsed / 1000 + " seconds !", _globalFont, 48));
            lines.Add(new Text("With " + _game.GetPlayer.GetLife.GetCurrentPoint + " HP.", _globalFont, 32));
            lines.Add(new Text("Press ENTER/A to QUIT", _globalFont, 32));

            for (int i = 0; i < lines.Count; i++)
            {
                RectangleShape r = new RectangleShape(new Vector2f(lines[i].GetGlobalBounds().Width + 20, lines[i].GetGlobalBounds().Height + 20));
                r.FillColor = Color.White;

                if (i == 0)
                    lines[i].FillColor = Color.Green;
                else if (i == lines.Count - 1)
                {
                    r.FillColor = Color.Black;
                    lines[i].FillColor = Color.White;
                }
                else
                    lines[i].FillColor = Color.Black;

                lines[i].Position = new SFML.System.Vector2f(_window.Size.X / 2 - (lines[i].GetGlobalBounds().Width) / 2, (_window.Size.Y / 6) * (i + 1));
                r.Position = new Vector2f(lines[i].Position.X - 10, lines[i].Position.Y);

                _window.Draw(r);
                _window.Draw(lines[i]);
            }

            _window.Display();

            bool quit = false;

            while (!quit)
            {
                Joystick.Update();
                if (Keyboard.IsKeyPressed(Keyboard.Key.Enter) || (Joystick.IsConnected(0) && Joystick.IsButtonPressed(0, 0)))
                    quit = true;
                System.Threading.Thread.Sleep(1);
            }

            // QUAND QUITTE LE MENU
            _menu = new Menu(_window);
            _settings = new Settings(this, _window);
            _gui = new GUI(this, _window);
            _timer = new Stopwatch();
            _timer.Start();
            _game = null;
        }

        public void WinMenu2()
        {
            View view1 = new View(new Vector2f(Settings.XResolution / 2, Settings.YResolution / 2), new Vector2f(Settings.XResolution, Settings.YResolution));
            view1.Viewport = new FloatRect(0f, 0f, 1f, 1f);
            view1.Size = new Vector2f(Settings.XResolution, Settings.YResolution);
            _window.SetView(view1);

            SFML.Graphics.Sprite background = new SFML.Graphics.Sprite(new Texture(@"..\..\..\..\Assets\Backgrounds\time-travel-background.png"));
            if (background == null) throw new Exception("Sprite null!");

            background.Scale = new SFML.System.Vector2f(_window.Size.X / 550, _window.Size.Y / 550);
            _window.Draw(background);

            List<Text> lines = new List<Text>();
            //Lines
            lines.Add(new Text("PLAYER 2 WIN !", _globalFont, 64));
            lines.Add(new Text("in : " + _game.TimeElapsed / 1000 + " seconds !", _globalFont, 48));
            lines.Add(new Text("With " + _game.GetPlayer.GetLife.GetCurrentPoint + " HP.", _globalFont, 32));
            lines.Add(new Text("Press ENTER/A to QUIT", _globalFont, 32));

            lines[0].FillColor = Color.Green;
            lines[1].FillColor = Color.Yellow;
            lines[2].FillColor = Color.Red;
            lines[3].FillColor = Color.Black;

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Position = new SFML.System.Vector2f(_window.Size.X / 2 - (lines[i].GetGlobalBounds().Width) / 2, (_window.Size.Y / 6) * i);
                _window.Draw(lines[i]);
            }

            _window.Display();

            bool quit = false;

            while (!quit)
            {
                Joystick.Update();
                if (Keyboard.IsKeyPressed(Keyboard.Key.Enter) || (Joystick.IsConnected(0) && Joystick.IsButtonPressed(0, 0)))
                    quit = true;
                System.Threading.Thread.Sleep(1);
            }

            // QUAND QUITTE LE MENU
            _menu = new Menu(_window);
            _settings = new Settings(this, _window);
            _gui = new GUI(this, _window);
            _timer = new Stopwatch();
            _timer.Start();
            _game = null;
        }

        public void DieMenu()
        {
            View view1 = new View(new Vector2f(Settings.XResolution / 2, Settings.YResolution / 2), new Vector2f(Settings.XResolution, Settings.YResolution));
            view1.Viewport = new FloatRect(0f, 0f, 1f, 1f);
            view1.Size = new Vector2f(Settings.XResolution, Settings.YResolution);
            _window.SetView(view1);

            SFML.Graphics.Sprite background = new SFML.Graphics.Sprite(new Texture(@"..\..\..\..\Assets\Backgrounds\time-travel-background.png"));
            if (background == null) throw new Exception("Sprite null!");

            _window.Draw(background);



            List<Text> lines = new List<Text>();
            //Lines
            lines.Add(new Text("PLAYER 1 DIIIIED !", _globalFont, 64));
            lines.Add(new Text("Killed by : " + _game.GetPlayer.KilledBy, _globalFont, 48));
            lines.Add(new Text("in : " + _game.TimeElapsed / 1000 + " seconds !", _globalFont, 32));
            lines.Add(new Text("Press ENTER to QUIT", _globalFont, 32));



            for (int i = 0; i < lines.Count; i++)
            {
                RectangleShape r = new RectangleShape(new Vector2f(lines[i].GetGlobalBounds().Width + 20, lines[i].GetGlobalBounds().Height + 20));
                r.FillColor = Color.White;

                if (i == 0)
                    lines[i].FillColor = Color.Red;
                else if (i == lines.Count - 1)
                {
                    r.FillColor = Color.Black;
                    lines[i].FillColor = Color.White;
                }
                else
                    lines[i].FillColor = Color.Black;

                lines[i].Position = new SFML.System.Vector2f(_window.Size.X / 2 - (lines[i].GetGlobalBounds().Width) / 2, (_window.Size.Y / 6) * (i + 1));
                r.Position = new Vector2f(lines[i].Position.X - 10, lines[i].Position.Y);

                _window.Draw(r);
                _window.Draw(lines[i]);
            }

            _window.Display();

            bool quit = false;

            while (!quit)
            {
                Joystick.Update();
                if (Keyboard.IsKeyPressed(Keyboard.Key.Enter) || (Joystick.IsConnected(0) && Joystick.IsButtonPressed(0, 0)))
                    quit = true;
                System.Threading.Thread.Sleep(1);
            }

            // QUAND QUITTE LE MENU
            _menu = new Menu(_window);
            _settings = new Settings(this, _window);
            _gui = new GUI(this, _window);
            _timer = new Stopwatch();
            _timer.Start();
            _game = null;
        }

        public void DieMenu2()
        {
            View view1 = new View(new Vector2f(Settings.XResolution / 2, Settings.YResolution / 2), new Vector2f(Settings.XResolution, Settings.YResolution));
            view1.Viewport = new FloatRect(0f, 0f, 1f, 1f);
            view1.Size = new Vector2f(Settings.XResolution, Settings.YResolution);
            _window.SetView(view1);

            SFML.Graphics.Sprite background = new SFML.Graphics.Sprite(new Texture(@"..\..\..\..\Assets\Backgrounds\time-travel-background.png"));
            if (background == null) throw new Exception("Sprite null!");

            background.Scale = new SFML.System.Vector2f(_window.Size.X / 550, _window.Size.Y / 550);
            _window.Draw(background);

            List<Text> lines = new List<Text>();
            //Lines
            lines.Add(new Text("PLAYER 2 DIEEEEED", _globalFont, 64));
            lines.Add(new Text("Killed by : " + _game2.GetPlayer2.KilledBy2, _globalFont, 48));
            lines.Add(new Text("in : " + _game.TimeElapsed / 1000 + " seconds !", _globalFont, 32));
            lines.Add(new Text("Press ENTER/A to QUIT", _globalFont, 32));

            lines[0].FillColor = Color.Green;
            lines[1].FillColor = Color.Yellow;
            lines[2].FillColor = Color.Red;
            lines[3].FillColor = Color.Black;

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Position = new SFML.System.Vector2f(_window.Size.X / 2 - (lines[i].GetGlobalBounds().Width) / 2, (_window.Size.Y / 6) * i);
                _window.Draw(lines[i]);
            }

            _window.Display();

            bool quit = false;

            while (!quit)
            {
                Joystick.Update();
                if (Keyboard.IsKeyPressed(Keyboard.Key.Enter) || (Joystick.IsConnected(0) && Joystick.IsButtonPressed(0, 0)))
                    quit = true;
                System.Threading.Thread.Sleep(1);
            }

            // QUAND QUITTE LE MENU
            _menu = new Menu(_window);
            _settings = new Settings(this, _window);
            _gui = new GUI(this, _window);
            _timer = new Stopwatch();
            _timer.Start();
            _game = null;
        }

        private string StringBetweenString(string original, string str1, string str2)
        {
            int firstStringPosition = original.IndexOf(str1);
            int secondStringPosition = original.IndexOf(str2);
            return original.Substring(firstStringPosition + str1.Length + 2, secondStringPosition - firstStringPosition - str2.Length);
        }

        internal void MusicStop(Music music)
        {
            if (music != null && music.Status == SoundStatus.Playing)
            {
                music.Stop();
                music.Dispose();
                _musicMenu.Play();
            }
        }

        public Menu GetMenu
        {
            get => _menu;
        }
        public Game GetGame
        {
            get => _game;
        }
        public Game GetGame2
        {
            get => _game2;
        }
        public Settings GetSettings
        {
            get => _settings;
        }
        public GUI GetGUI
        {
            get => _gui;
        }

        public bool Close
        {
            get => CLOSE;
            set => CLOSE = value;
        }

        internal Stopwatch Timer
        {
            get => _timer;
        }
        public Checkpoint GetCheckpoint
        {
            get => _checkpoint;
        }

        public Font GetFont
        {
            get => _globalFont;
        }
        internal Music GetMusicMenu
        {
            get => _musicMenu;
        }
    }
}
