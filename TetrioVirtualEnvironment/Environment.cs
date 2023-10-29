using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using TetrioVirtualEnvironment.Constants;
using TetrioVirtualEnvironment.Env;
using TetrLoader.Enum;
using TetrLoader.JsonClass;
using TetrLoader.JsonClass.Event;

namespace TetrioVirtualEnvironment
{
	public class Environment
	{
		public const int FIELD_WIDTH = 10;
		public const int FIELD_HEIGHT = 40;

		/// <summary>
		/// Event Callback Managed Class
		/// </summary>
		public EventManager EventManager;

		public Stats Stats { get; private set; }
		public CustomStats CustomStats { get; private set; }
		public readonly GameData GameData;
		private Rng Rng { get; }

		private List<int>? GarbageIDs { get; set; }

		public readonly string Username;

		/// <summary>
		/// RNG Generator
		/// </summary>

		public NextGenerateType NextGenerateMode { get; }

		public int NextSkipCount;
		public int GeneratedRngCount { get; private set; }

		/// <summary>
		/// for process events at same frame.
		/// </summary>
		public int CurrentIndex { get; private set; }

		public int CurrentFrame { get; private set; }
		public bool IsDead { get; internal set; } = false;
		public readonly bool InfinityHold;

		/// <summary>
		/// event data of 'full'.use it for reset the game.
		/// full event is first event after game started existing in ttr/ttrm.
		/// </summary>
		internal readonly EventFullData EventFull;

		/// <summary>
		/// initialize game data.
		/// </summary>
		internal FieldData InitializeData { get; }

		/// <summary>
		/// ArrayMode only use initialized Next.
		/// SeedMode generate next from seed based on TETR.IO generate system.
		/// </summary>
		public enum NextGenerateType : byte
		{
			Array,
			Seed
		}


		/// <summary>
		/// for AI moving
		/// </summary>
		public enum Action : byte
		{
			Null,
			MoveUp,
			MoveDown,
			MoveRight,
			MoveLeft,
			RotateRight,
			RotateLeft,
			Rotate180,
			Harddrop,
			QuickSoftdrop,
			Softdrop,
			Hold
		}

		public enum KeyEvent
		{
			KeyDown,
			KeyUp
		}

		internal BoardInfo BoardInfo;
		internal FallingOprations FallingOprations;
		internal Env.Garbage Garbage;
		internal Judge Judge;

		internal ServiceProvider Provider;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventFull"></param>
		/// <param name="nextGenType"></param>
		/// <param name="username"></param>
		/// <param name="infinityHold"></param>
		/// <param name="initializeData"></param>
		/// <param name="nextSkipCount"></param> 
		public Environment(EventFullData eventFull, NextGenerateType nextGenType, string? username,
			bool infinityHold = false,
			FieldData? initializeData = null, int nextSkipCount = 0)
		{
			InfinityHold = infinityHold;

			initializeData ??= new FieldData();
			EventFull = eventFull;
			InitDependencyInjection(eventFull);
			if (Provider == null)
				throw new NullReferenceException();


			BoardInfo = Provider.GetService<BoardInfo>() ?? throw new InvalidOperationException();
			FallingOprations = Provider.GetService<FallingOprations>() ?? throw new InvalidOperationException();
			Judge = Provider.GetService<Judge>() ?? throw new InvalidOperationException();
			Garbage = Provider.GetService<Env.Garbage>() ?? throw new InvalidOperationException();
			GameData = Provider.GetService<GameData>() ?? throw new InvalidOperationException();


			Rng = new Rng();
			Username = username;
			NextGenerateMode = nextGenType;
			InitializeData = initializeData;
			NextSkipCount = nextSkipCount;

			ResetGame(Provider.GetService<EventFullData>(), nextGenType,
				initializeData, nextSkipCount);
		}

		private void InitDependencyInjection(EventFullData eventFull)
		{
			ServiceCollection service = new ServiceCollection();
			service.AddSingleton<GameData, GameData>();
			service.AddSingleton<EventFullData>(eventFull);
			service.AddSingleton<Environment>(this);
			service.AddSingleton<Falling>();
			service.AddSingleton<BoardInfo>();
			service.AddSingleton<FallingOprations>();
			service.AddSingleton<Judge>();
			service.AddSingleton<Env.Garbage>();
			service.AddSingleton(prov =>
			{
				var data = prov.GetRequiredService<EventFullData>();
				return new Options(data.options ?? throw new InvalidOperationException());
			});

			Provider = service.BuildServiceProvider();
		}

		/// <summary>
		/// ResetGame for rewind
		/// </summary>
		/// <param name="eventFull"></param>
		/// <param name="nextGenType"></param>
		/// <param name="dataForInitialize"></param>
		/// <param name="nextSkipCount"></param>
		internal void ResetGame(EventFullData eventFull, NextGenerateType nextGenType, FieldData dataForInitialize,
			int nextSkipCount = 0)
		{
			EventManager = new EventManager();

			Rng.Init(eventFull.options.seed);
			Stats = new Stats();
			CustomStats = new CustomStats();


			IsDead = false; 
			GeneratedRngCount = 0;
			GarbageIDs = new List<int>();
			CurrentFrame = 0;
			CurrentIndex = 0;

			GameData.Init(nextGenType, eventFull, nextSkipCount, dataForInitialize);
		}


		public void KeyInput(EventType? type, EventKeyInputData @event)
		{
			if (type == EventType.Keydown)
			{
				if (@event.subframe > GameData.SubFrame)
				{
					FallingOprations.ProcessShift(false, @event.subframe - GameData.SubFrame);
					FallEvent(null, @event.subframe - GameData.SubFrame);
					GameData.SubFrame = @event.subframe;
				}

				if (@event.key == KeyType.MoveLeft)
				{
					GameData.LShift = true;
					GameData.LastShift = -1;
					GameData.LDas = @event.hoisted ? GameData.Handling.DAS - GameData.Handling.DCD : 0;
					if (GameData.Options.Version >= 12)
						GameData.LDasIter = GameData.Handling.ARR;

					FallingOprations.ProcessLShift(true, GameData.Options.Version >= 15 ? 0 : 1);
					return;
				}

				if (@event.key == KeyType.MoveRight)
				{
					GameData.RShift = true;
					GameData.LastShift = 1;
					GameData.RDas = @event.hoisted ? GameData.Handling.DAS - GameData.Handling.DCD : 0;
					if (GameData.Options.Version >= 12)
						GameData.RDasIter = GameData.Handling.ARR;

					FallingOprations.ProcessRShift(true, GameData.Options.Version >= 15 ? 0 : 1);
					return;
				}

				if (@event.key == KeyType.SoftDrop)
				{
					GameData.SoftDrop = true;
					return;
				}

				if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
				{
					throw new NotImplementedException();
				}
				else
				{
					if (@event.key == KeyType.RotateCCW)
					{
						var e = GameData.Falling.R - 1;
						if (e < 0)
							e = 3;
						FallingOprations.RotatePiece(e);
					}

					if (@event.key == KeyType.RotateCW)
					{
						var e = GameData.Falling.R + 1;
						if (e > 3)
							e = 0;
						FallingOprations.RotatePiece(e);
					}

					if (@event.key == KeyType.Rotate180 && GameData.Options.Allow180)
					{
						var e = GameData.Falling.R + 2;
						if (e > 3)
							e -= 4;
						FallingOprations.RotatePiece(e);
					}

					if (@event.key == KeyType.HardDrop && GameData.Options.AllowHardDrop &&
					    GameData.Falling.SafeLock == 0)
					{
						FallEvent(int.MaxValue, 1);
					}

					if (@event.key == KeyType.Hold)
					{
						if (!GameData.HoldLocked || InfinityHold)
						{
							if ((GameData.Options.DisplayHold == null ||
							     (bool)GameData.Options.DisplayHold))
								SwapHold();
						}
					}
				}
			}
			else if (type == EventType.Keyup)
			{
				if (@event.subframe > GameData.SubFrame)
				{
					FallingOprations.ProcessShift(false, @event.subframe - GameData.SubFrame);
					FallEvent(null, @event.subframe - GameData.SubFrame);
					GameData.SubFrame = @event.subframe;
				}

				if (@event.key == KeyType.MoveLeft)
				{
					GameData.LShift = false;
					GameData.LDas = 0;

					if (GameData.Handling.Cancel)
					{
						GameData.RDas = 0;
						GameData.RDasIter = GameData.Handling.ARR;
					}

					return;
				}

				if (@event.key ==KeyType.MoveRight)
				{
					GameData.RShift = false;
					GameData.RDas = 0;

					if (GameData.Handling.Cancel)
					{
						GameData.LDas = 0;
						GameData.LDasIter = GameData.Handling.ARR;
					}

					return;
				}

				if (@event.key == KeyType.SoftDrop)
					GameData.SoftDrop = false;
			}
		}


		/// <summary>
		/// Update game. Don't call if environment is managed by Replay class.
		/// </summary>
		/// <param name="events"></param>
		/// <returns></returns>
		public bool Update(IReadOnlyList<Event>? events = null)
		{
			GameData.SubFrame = 0;

			if (events != null && !PullEvent(events))
				return false;

			CurrentFrame++;
			FallingOprations.ProcessShift(false, 1 - GameData.SubFrame);
			FallEvent(null, 1 - GameData.SubFrame);

			if (events == null)
				return true;

			ExcuteWaitingFrames();


			if (GameData.Options.GravityIncrease > 0 &&
			    CurrentFrame > GameData.Options.GravityMargin)
				GameData.Gravity += GameData.Options.GravityIncrease / 60;

			if (GameData.Options.GarbageIncrease > 0 &&
			    CurrentFrame > GameData.Options.GarbageMargin)
				GameData.Options.GarbageMultiplier += GameData.Options.GarbageIncrease / 60;

			if (GameData.Options.GarbageCapIncrease > 0)
				GameData.Options.GarbageCap += GameData.Options.GarbageCapIncrease / 60;


			return true;
		}

		/// <summary>
		/// Update game during events[CurrentIndex].frame == CurrentFrame 
		/// </summary>
		/// <param name="events"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private bool PullEvent(IReadOnlyList<Event> events)
		{
			while (true)
			{
				if (events[CurrentIndex].frame == CurrentFrame)
				{
					switch (events[CurrentIndex].type)
					{
						case EventType.Start:
							break;

						case EventType.Full:
							GameData.Falling.Create(null, false, NextGenerateMode);
							break;

						case EventType.Keydown:
						case EventType.Keyup:
							var inputEvent = events[CurrentIndex] as EventKeyInput;

							if (inputEvent == null)
								throw new Exception("inputEvent is null.");

							KeyInput(inputEvent.type, inputEvent.data);

							break;


						case EventType.Targets:
							var targets = events[CurrentIndex] as EventTargets;
							for (int i = 0; i < targets.data.data.Count; i++)
								targets.data.data[i] = targets.data.data[i].Substring(0, 24);

							GameData.Targets = targets.data.data;

							break;

						case EventType.Ige:
							var garbageEvent = (EventIge)(events[CurrentIndex] as EventIge).Clone();


							if (garbageEvent.id != null)
								Debug.WriteLine("予期せぬEvent状態をしています。正常にリプレイが再生されない可能性があります。");


							if (GarbageIDs.Contains((int)garbageEvent.data.id))
								break;

							GarbageIDs.Add((int)garbageEvent.data.id);

							switch (garbageEvent.data.data.type)
							{
								case "attack":
									Garbage.IncomingAttack(new GarbageData()
									{
										type = "garbage",
										amt = garbageEvent.data.data.lines,
										column = garbageEvent.data.data.column,
										x = -1,
										y = -1,
									}, garbageEvent.data.data.sender, garbageEvent.data.data.cid.ToString());
									break;

								case "interaction":

									switch (garbageEvent.data.data.data.type)
									{
										case "garbage":
											string? idValue;
											if (GameData.Options.ClipListenIDs)
												idValue = garbageEvent.data.data.sender_id != null
													? garbageEvent.data.data.sender_id.Substring(0, 24)
													: null;
											else
												idValue = garbageEvent.data.data.sender_id;
											Garbage.StartingAttack(garbageEvent.data.data.data,
												garbageEvent.data.data.sender,
												idValue, garbageEvent.data.data.cid);

											break;
									}

									break;

								case "interaction_confirm":
									switch (garbageEvent.data.data.data.type)
									{
										case "garbage":
											string id;
											if (GameData.Options.ClipListenIDs)
												id = garbageEvent.data.data.sender_id != null
													? garbageEvent.data.data.sender_id.Substring(0, 24)
													: null;
											else
												id = garbageEvent.data.data.sender_id;

											Garbage.IncomingAttack(garbageEvent.data.data.data,
												garbageEvent.data.data.sender,
												id, garbageEvent.data.data.cid);
											break;
									}

									break;
								case "kev":
									break;
							}

							break;

						case EventType.End:
							return false;

						default:
							throw new Exception("unknown event type: " + events[CurrentIndex].type);
					}

					CurrentIndex++;
				}
				else break;
			}


			return true;
		}


		internal void WaitFrames(int waitFrame, string type, object data)
		{
			GameData.WaitingFrames.Add(new WaitingFrameData()
			{
				target = CurrentFrame + waitFrame,
				type = type,
				AdditionalData = data
			});
		}

		private void ExcuteWaitingFrames()
		{
			for (int waitingFrameIndex = GameData.WaitingFrames.Count - 1; waitingFrameIndex >= 0; waitingFrameIndex--)
			{
				if (CurrentFrame == GameData.WaitingFrames[waitingFrameIndex].target)
				{
					ExcuteWaitingFrame(GameData.WaitingFrames[waitingFrameIndex]);
					GameData.WaitingFrames.RemoveAt(waitingFrameIndex);
				}
			}
		}

		private void ExcuteWaitingFrame(in WaitingFrameData data)
		{
			if (data.type == "incoming-attack-hit")
			{
				var igedata = (IgeData)data.AdditionalData;
				Garbage.IncomingAttackHit(igedata.data, igedata.sender, igedata.cid);
			}
			else if (data.type == "outgoing-attack-hit")
			{
				// OutgoingAttackHit();
			}
			else if (data.type == "are")
			{
				throw new NotImplementedException();
			}
			else if (data.type == "freeze-counters")
			{
				throw new NotImplementedException();
			}
			else if (data.type == "revive-from-stock-loss")
			{
				throw new NotImplementedException();
			}
			else
			{
				throw new Exception("unknown type");
			}
		}


		internal int GetAmt(GarbageData data, string? id = null)
		{
			GameData.GarbageActnowledgements.Incoming[id] = data.iid;

			var amt = data.amt;

			if (GameData.GarbageActnowledgements.Outgoing.ContainsKey(id))
			{
				var garbageList = new List<GarbageData?>();

				foreach (var outgoing in GameData.GarbageActnowledgements.Outgoing[id])
				{
					if (outgoing.iid <= data.ackiid)
						continue;
					var minAmt = Math.Min(outgoing.amt, amt);
					outgoing.amt -= minAmt;
					amt -= minAmt;


					if (outgoing.amt > 0)
						garbageList.Add(outgoing);
				}

				GameData.GarbageActnowledgements.Outgoing[id] = garbageList;
				
			}

			return amt;
		}


		private void FallEvent(int? value, double subFrameDiff)
		{
			if (GameData.Falling.SafeLock > 0)
				GameData.Falling.SafeLock--;

			if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
				return;

			var subfFrameGravity = GameData.Gravity * subFrameDiff;

			if (GameData.SoftDrop)
			{
				if (GameData.Handling.SDF == (GameData.Options.Version >= 15 ? 41 : 21))
					subfFrameGravity = (GameData.Options.Version >= 15 ? 400 : 20) * subFrameDiff;
				else
				{
					subfFrameGravity *= GameData.Handling.SDF;
					subfFrameGravity = Math.Max(subfFrameGravity,
						GameData.Options.Version >= 13 ? 0.05 * GameData.Handling.SDF : 0.42);
				}
			}

			if (value != null)
				subfFrameGravity = (int)value;

			if (!GameData.Options.InfiniteMovement &&
			    GameData.Falling.LockResets >= (int)GameData.Options.LockResets &&
			    !Judge.IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X,
				    GameData.Falling.Y + 1, GameData.Falling.R, GameData.Board))
			{
				subfFrameGravity = 20;
				GameData.Falling.ForceLock = true;
			}


			if (!GameData.Options.InfiniteMovement &&
			    GameData.FallingRotations > (int)GameData.Options.LockResets + 15)
			{
				subfFrameGravity += 0.5 * subFrameDiff *
				                    (GameData.FallingRotations - ((int)GameData.Options.LockResets + 15));
			}

			double constSubFrameGravity = subfFrameGravity;

			for (; subfFrameGravity > 0;)
			{
				var yPos = (int)Math.Ceiling(GameData.Falling.Y);
				if (!HardDrop(Math.Min(1, subfFrameGravity), constSubFrameGravity))
				{
					if (value != null)
						GameData.Falling.ForceLock = true;
					Locking(value != 0 && value != null, subFrameDiff);
					break;
				}

				subfFrameGravity -= Math.Min(1, subfFrameGravity);
				if (yPos != (int)Math.Ceiling(GameData.Falling.Y))
				{
					GameData.Falling.Last = Falling.LastKind.Fall;
					if (GameData.SoftDrop)
					{
						//ScoreAdd
					}
				}
			}
		}

		private bool HardDrop(double value, double value2)
		{
			var yPos1 = Math.Floor(Math.Pow(10, 13) * GameData.Falling.Y) /
				Math.Pow(10, 13) + value;

			if (yPos1 % 1 == 0)
				yPos1 += 0.00001;

			var yPos2 = Math.Floor(Math.Pow(10, 13) * GameData.Falling.Y) / Math.Pow(10, 13) + 1;
			if (yPos2 % 1 == 0)
				yPos2 -= 0.00002;

			if (Judge.IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X, yPos1, GameData.Falling.R,
				    GameData.Board) &&
			    Judge.IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X, yPos2, GameData.Falling.R,
				    GameData.Board))
			{
				var highestY = GameData.Falling.HighestY;
				yPos2 = GameData.Falling.Y;

				GameData.Falling.Y = yPos1;
				GameData.Falling.HighestY = Math.Ceiling(Math.Max(GameData.Falling.HighestY, yPos1));
				GameData.Falling.Floored = false;
				if ((int)Math.Ceiling(yPos1) != (int)Math.Ceiling(yPos2))
				{
				}

				if (yPos1 > highestY || GameData.Options.InfiniteMovement)
					GameData.Falling.LockResets = 0;
				GameData.FallingRotations = 0;

				if (value2 >= int.MaxValue)
				{
					//スコア足す
				}

				return true;
			}

			return false;
		}

		private void Lock(bool sValue, bool emptyDrop = false)
		{
			GameData.Falling.Sleep = true;
			//ミノを盤面に適用
			if (!emptyDrop)
			{
				foreach (var pos in Tetrimino.SHAPES[(int)GameData.Falling.Type][GameData.Falling.R])
				{
					GameData.Board[(int)((pos.x + GameData.Falling.X -
					                      Tetrimino.DIFFS[(int)GameData.Falling.Type].x) +
					                     (int)(pos.y + GameData.Falling.Y -
					                           Tetrimino.DIFFS[(int)GameData.Falling.Type].y) * 10)] =
						GameData.Falling.Type;
				}

				if (!sValue && GameData.Handling.SafeLock > 0)
					GameData.Falling.SafeLock = 7;
			}

			var isTspin = Judge.IsTspin();

			EventManager.Trigger_OnPiecePlaced(this, EventArgs.Empty);
			CustomStats.TotalPiecePlaced++;

			var clearedLineCount = BoardInfo.ClearLines(out var garbageClear, out var stackClear);

			CustomStats.TotalGarbageClear += garbageClear;
			CustomStats.TotalStackClear += stackClear;

			var announceLine = BoardInfo.AnnounceLines(clearedLineCount, isTspin);


			BoardInfo.AnnounceClear();


			if (!announceLine)
				Garbage.TakeAllDamage();


			GameData.Falling.Create(null, false, NextGenerateMode);
		}

		internal void CallOnPieceCreated()
		{
			EventManager.Trigger_OnPieceCreated(this, EventArgs.Empty);
		}


		public void UserInput(Action action)
		{
			switch (action)
			{
				case Action.MoveRight:
					if (Judge.IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X + 1,
						    GameData.Falling.Y, GameData.Falling.R, GameData.Board))
						GameData.Falling.X++;
					break;

				case Action.MoveLeft:
					if (Judge.IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X - 1,
						    GameData.Falling.Y, GameData.Falling.R, GameData.Board))
						GameData.Falling.X--;
					break;

				case Action.RotateLeft:
					var e = GameData.Falling.R - 1;
					if (e < 0)
						e = 3;
					FallingOprations.RotatePiece(e);
					break;

				case Action.RotateRight:
					e = GameData.Falling.R + 1;
					if (e > 3)
						e = 0;
					FallingOprations.RotatePiece(e);
					break;

				case Action.Rotate180:
					e = GameData.Falling.R + 2;
					if (e > 3)
						e -= 4;
					FallingOprations.RotatePiece(e);
					break;

				case Action.Hold:
					SwapHold();
					break;

				case Action.Harddrop:
					FallEvent(int.MaxValue, 1);
					break;

				case Action.QuickSoftdrop:
					while (true)
					{
						if (Judge.IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X,
							    GameData.Falling.Y + 1, GameData.Falling.R, GameData.Board))
							GameData.Falling.Y++;
						else
							break;
					}

					break;

				case Action.MoveUp:
					if (Judge.IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X,
						    GameData.Falling.Y - 1, GameData.Falling.R, GameData.Board))
						GameData.Falling.Y--;
					break;

				case Action.MoveDown:
					if (Judge.IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X,
						    GameData.Falling.Y + 1, GameData.Falling.R, GameData.Board))
						GameData.Falling.Y++;
					break;

				default: throw new Exception();
			}
		}


		public static Vector2[] GetMinoPos(Tetrimino.MinoType type, int x, int y, int r)
		{
			var positions = (Vector2[])Tetrimino.SHAPES[(int)type][r].Clone();
			var diff = Tetrimino.DIFFS[(int)type];

			for (int i = 0; i < positions.Length; i++)
			{
				positions[i].x += x - diff.x;
				positions[i].y += y - diff.y;
			}

			return positions;
		}

		public int GetFallestPosDiff()
		{
			return GetFallestPosDiff(GameData.Falling.Type, GameData.Falling.X, (int)GameData.Falling.Y,
				GameData.Falling.R);
		}

		public int GetFallestPosDiff(Tetrimino.MinoType type, int x, int y, int r)
		{
			if (type == Tetrimino.MinoType.Empty)
				return 0;

			int testY = 1;
			while (true)
			{
				if (Judge.IsLegalAtPos(type, x, y + testY,
					    r, GameData.Board))
					testY++;
				else
				{
					testY--;
					break;
				}
			}

			return testY;
		}


		/// <summary>
		/// 消去したラインを下げる
		/// </summary>
		/// <param name="value">y座標</param>
		/// <param name="field">盤面</param>
		internal void DownLine(int value, Tetrimino.MinoType[] field)
		{
			for (int y = value; y >= 0; y--)
			{
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (y - 1 == -1)
						field[x + y * 10] = Tetrimino.MinoType.Empty;
					else
						field[x + y * 10] = field[x + (y - 1) * 10];
				}
			}
		}

		/// <summary>
		/// Place piece if threshold is over.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="subframe"></param>
		private void Locking(bool value = false, double subframe = 1)
		{
			if (GameData.Options.Version >= 15)
				GameData.Falling.Locking += subframe;
			else
				GameData.Falling.Locking += 1;

			if (!GameData.Falling.Floored)
				GameData.Falling.Floored = true;


			if (GameData.Falling.Locking > GameData.Options.LockTime ||
			    GameData.Falling.ForceLock ||
			    GameData.Falling.LockResets > GameData.Options.LockResets &&
			    !GameData.Options.InfiniteMovement)
			{
				Lock(value);
			}
		}

		internal Tetrimino.MinoType RefreshNext(Queue<Tetrimino.MinoType> nextQueue, bool noszo)
		{
			GeneratedRngCount++;

			var newType = nextQueue.Dequeue();


			if (GameData.NextBag.Count == 0)
			{
				var array = (Tetrimino.MinoType[])Tetrimino.MINOTYPES.Clone();
				Rng.ShuffleArray(array);
				GameData.NextBag.AddRange(array);
			}

			if (noszo)
			{
				while (true)
				{
					if (GameData.NextBag[0] == Tetrimino.MinoType.S ||
					    GameData.NextBag[0] == Tetrimino.MinoType.Z ||
					    GameData.NextBag[0] == Tetrimino.MinoType.O)
					{
						var temp = GameData.NextBag[0];
						for (int i = 0; i < GameData.NextBag.Count - 1; i++)
							GameData.NextBag[i] = GameData.NextBag[i + 1];

						GameData.NextBag[^1] = temp;
					}
					else
						break;
				}
			}


			nextQueue.Enqueue(GameData.NextBag[0]);
			GameData.NextBag.RemoveAt(0);
			return newType;
		}

		private void SwapHold()
		{
			var tempCurrentType = GameData.Falling.Type;
			GameData.Falling.Create(GameData.Hold, true, NextGenerateMode);
			GameData.Hold = tempCurrentType;
		}
	}
}