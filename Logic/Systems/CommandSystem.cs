using Game.Core;
using Game.Core.Entities.Monsters;
using Game.Core.Items;
using Game.Interfaces;
using Game.Logic.MapGeneration;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using System;
using System.Text;

namespace Game.Logic
{
    public class CommandSystem
    {
        // Return value is true if the player was able to move
        // false is returned when the player couldn't move, i.e. trying to move into a wall


        public bool IsPlayerTurn { get; set; }

        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
        }

        public bool GetKeyPress(RLKeyPress keyPress)
        {
            if (keyPress != null)
            {
                if (keyPress.Key == RLKey.Up)
                {
                    return MovePlayer(Direction.Up);
                }
                else if (keyPress.Key == RLKey.Down)
                {
                    return MovePlayer(Direction.Down);
                }
                else if (keyPress.Key == RLKey.Left)
                {
                    return MovePlayer(Direction.Left);
                }
                else if (keyPress.Key == RLKey.Right)
                {
                    return MovePlayer(Direction.Right);
                }
                else if (keyPress.Key == RLKey.Escape)
                {
                    Game.rootConsole.Close();
                }
                else if (keyPress.Key == RLKey.Period)
                {
                    if (Game.DungeonMap.CanMoveDownToNextLevel())
                    {
                        MapGenerator mapGenerator = new MapGenerator(Game.mapWidth, Game.mapHeight, 20, 13, 7, ++Game.mapLevel);
                        Game.DungeonMap = mapGenerator.CreateMap(Game.mapConsole);
                        Game.MessageLog = new MessageLog();
                        Game.CommandSystem = new CommandSystem();
                        Game.rootConsole.Title = $"Game - Level {Game.mapLevel}";
                        Game.MessageLog.Add($"You reached {Game.mapLevel} level!");
                        return true;
                    }
                }
                else if (keyPress.Key == RLKey.P) // shortcut to get more scrolls for testing
                {
                    int? x = null;
                    int? y = null;
                    PlayerInventory.AddToQuickBar(new ScrollOfDestruction(x, y));
                }
                else if (keyPress.Key == RLKey.O)
                {
                    int? x = null;
                    int? y = null;
                    PlayerInventory.AddToQuickBar(new ScrollOfTeleport(x, y));
                }
                else if (keyPress.Key == RLKey.Number1)
                {
                    return UseItem(0);
                }
                else if (keyPress.Key == RLKey.Number2)
                {
                    return UseItem(1);
                }
                else if(keyPress.Key == RLKey.Number3)
                {
                    return UseItem(2);
                }

                
            }
            return false;
        }

        public bool UseItem(int key)
        {
            if (Player.Inventory.Actives.Count == 0)
            {
                return false;
            }
            try
            {
                switch (key)
                {
                    case 0:
                        {
                            Player.Inventory.Actives[0].Use();
                            return true;
                        }
                    case 1:
                        {
                            Player.Inventory.Actives[1].Use();
                            return true;
                        }
                    case 2:
                        {
                            Player.Inventory.Actives[2].Use();
                            return true;
                        }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Game.MessageLog.Add("No item to use");
                return false;
            }
            return false;
        }

        public void ActivateMonsters()
        {
            IScheduleable scheduleable = Game.SchedulingSystem.Get();
            if(scheduleable is Player)
            {
                IsPlayerTurn = true;
                Game.SchedulingSystem.Add(Game.Player);
            }
            else
            {
                Monster monster = scheduleable as Monster;
                if(monster != null)
                {
                    monster.PerformAction(this);
                    Game.SchedulingSystem.Add(monster);
                }

                ActivateMonsters();
            }
        }

        public void MoveMonster(Monster monster, Cell cell)
        {
            if (!Game.DungeonMap.SetActorPostion(monster, cell.X, cell.Y))
            {
                if (monster.IsInRange())
                {
                    Attack(monster, Game.Player);
                }
                else if (Game.Player.X == cell.X && Game.Player.Y == cell.Y)
                {
                    Attack(monster, Game.Player);
                }
            }
        }

        public bool MovePlayer(Direction direction)
        {
            int x = Game.Player.X;
            int y = Game.Player.Y;

            switch(direction)
            {
                case Direction.Up:
                    {
                        y = Game.Player.Y - 1;
                        break;
                    }
                case Direction.Down:
                    {
                        y = Game.Player.Y + 1;
                        break;
                    }
                case Direction.Left:
                    {
                        x = Game.Player.X - 1;
                        break;
                    }
                case Direction.Right:
                    {
                        x = Game.Player.X + 1;
                        break;
                    }
                default:
                    {
                        return false;
                    }
            }

            Game.DungeonMap.SetActorPostion(Game.Player, x, y);

            Monster monster = Game.DungeonMap.GetMonsterAt(x, y);

            if(monster != null)
            {
                Attack(Game.Player, monster);
                return true;
            }

            return false;
        }

        public void Attack(Actor attacker, Actor defender)
        {
            StringBuilder attackMessage = new StringBuilder();
            StringBuilder defenseMessage = new StringBuilder();

            int hits = ResolveAttack(attacker, defender, attackMessage);

            int blocks = ResolveDefense(defender, hits, attackMessage, defenseMessage);

            Game.MessageLog.Add(attackMessage.ToString());
            if(!string.IsNullOrWhiteSpace(defenseMessage.ToString()))
            {
                Game.MessageLog.Add(defenseMessage.ToString());
            }

            int damage = hits - blocks;

            ResolveDamage(defender, damage);
        }
        
        // Roll the number of hits
        private static int ResolveAttack(Actor attacker, Actor defender, StringBuilder attackMessage)
        {
            int hits = 0;

            attackMessage.AppendFormat("{0} attacks {1} for ", attacker.Name, defender.Name);

            // Roll a number of 100-sided dice equall to the the Attack val of the attacking actor
            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100);
            DiceResult attackResult = attackDice.Roll();

            // Check the value of each singe rolled dice
            foreach(TermResult termResult in attackResult.Results)
            {
                //attackMessage.Append(termResult.Value + ", ");

                if(termResult.Value >= 100 - attacker.AttackChance)
                {
                    hits++;
                }

            }
            return hits;
        }

        // The defender rolls basen on his stats to see if he blocks any of the hits from the attacker
        private static int ResolveDefense(Actor defender, int hits, StringBuilder attackMessage, StringBuilder defenseMessage)
        {
            int blocks = 0;

            if(hits > 0)
            {
                attackMessage.AppendFormat("{0} hits.", hits);
                defenseMessage.AppendFormat(" {0} is defending ", defender.Name);

                // Roll a number of 100-sided dice equal to the Defense value of the defendering actor
                DiceExpression defenseDice = new DiceExpression().Dice(defender.Defense, 100);
                DiceResult defenseRoll = defenseDice.Roll();

                // Check the value of each singe rolled dice
                foreach(TermResult termResult in defenseRoll.Results)
                {
                    //defenseMessage.Append(termResult.Value + ", ");

                    if(termResult.Value >= 100 - defender.DefenseChance)
                    {
                        blocks++;
                    }
                }
                defenseMessage.AppendFormat("resulting in {0} blocks.", blocks);

            }
            else
            {
                attackMessage.Append(" and misses completly.");
            }


            return blocks;
        }

        private static void ResolveDamage(Actor defender, int damage)
        {
            if(damage > 0)
            {
                defender.Health -= damage;

                Game.MessageLog.Add($"  {defender.Name} was hit for {damage} damage.");
                if(defender.Health <= 0)
                {
                    ResolveDeath(defender);
                }
            }
            else
            {
                Game.MessageLog.Add($"  {defender.Name} blocked all damage.");
            }
        }

        private static void ResolveDeath(Actor actor)
        {
            if(actor is Player)
            {
                Game.MessageLog.Add("You just died. Game over!");
            }
            else if(actor is Monster)
            {
                Game.DungeonMap.RemoveMonster((Monster)actor);
                RandomDrop.Next((Monster)actor);
            }
        }


    }
}
