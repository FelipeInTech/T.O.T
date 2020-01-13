﻿using System;
using System.IO;
using System.Text;
using System.Diagnostics;



namespace TripOverTime.EngineNamespace
{
    internal class Attack
    {
        Game _context;
        Monster _monster;
        Boss _boss;
        ushort _attack;
        Stopwatch _timer;
        string[] _schemaAttack;
        int _incrementationAttack;
        string _mapPath;

        internal Attack(Game context, Boss boss, ushort attack)
            :this (context, null, boss, attack) { }
        internal Attack(Game context, Monster monster, ushort attack)
            : this(context, monster, null, attack) { }
        internal Attack(Game context, Monster monster, Boss boss, ushort attack)
        {
            _context = context;
            _monster = monster;
            _boss = boss;
            _attack = attack;
            _timer = new Stopwatch();
            _timer.Start();
            _mapPath = @"..\..\..\..\Maps\attackMonster.totMonster";

            //Verify if it's a map file
            if (!_mapPath.EndsWith(".totMonster")) throw new ArgumentException("The map file is not correct (.totMonster)");
            string text = File.ReadAllText(_mapPath);
            // Open map file
            if (String.IsNullOrEmpty(text)) throw new FileLoadException("File is empty ?");
            if (_monster != null)
            {
                _schemaAttack = StringBetweenString(text, _monster.Name + "UP", _monster.Name + "END").Split("\n");
            }
            else
            {
                _schemaAttack = StringBetweenString(text, _boss.Name + "UP", _boss.Name + "END").Split("\n");
            }
            _incrementationAttack = 0;
        }

        internal void AttackMonster()
        {
            if (_incrementationAttack == _schemaAttack[0].Length -1 )
            {
                _incrementationAttack = 0;
            }

            if (_schemaAttack[0][_incrementationAttack] == 'S')
            { 
                SlidingAttack();
                if (_timer.ElapsedMilliseconds >= 700 && _monster.Position.X + 2 > _context.GetPlayer.RealPosition.X && _monster.Position.X - 2 < _context.GetPlayer.RealPosition.X && _monster.Position.Y == _context.GetPlayer.RealPosition.Y && _monster.IsAlive)
                {
                    HurtPlayer();
                    _incrementationAttack++;
                    _timer.Restart();
                }
            }
            else if (_schemaAttack[0][_incrementationAttack] == 'A')
            {
                NormalAttack();

                if (_timer.ElapsedMilliseconds >= 910 && _monster.Position.X + 2 > _context.GetPlayer.RealPosition.X && _monster.Position.X - 2 < _context.GetPlayer.RealPosition.X && _monster.Position.Y == _context.GetPlayer.RealPosition.Y && _monster.IsAlive)
                {
                    HurtPlayer();
                    _incrementationAttack++;
                    _timer.Restart();
                }
            }

        }

        internal void AttackBoss()
        { }


        internal void NormalAttack()
        {
            _monster.GetMonsterSprite.MonsterAttackAnimation(12, "attack", _monster);
        }

        internal void SlidingAttack()
        {
            _monster.GetMonsterSprite.MonsterAttackAnimation(6, "sliding", _monster);
        }
        internal void HurtPlayer ()
        {
            _context.GetPlayer.GetLife.DecreasedPoint(_attack);
            _context.GetPlayer.HurtPlayer = true;
            HurtPlayerAnimation();
        }

        internal void HurtPlayerAnimation()
        {
            _context.GetPlayer.GetPlayerSprite.PlayerAnimation(4, "hurt", 60);
        }

        private string StringBetweenString(string original, string str1, string str2)
        {
            int firstStringPosition = original.IndexOf(str1);
            int secondStringPosition = original.IndexOf(str2);
            return original.Substring(firstStringPosition + str1.Length + 2, secondStringPosition - firstStringPosition - str2.Length);
        }

        internal ushort GetAttack
        {
            get => _attack;
            set => _attack = value;
        }
    }
}
