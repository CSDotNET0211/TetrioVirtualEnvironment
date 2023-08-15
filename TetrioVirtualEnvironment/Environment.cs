using TetrioVirtualEnvironment.Constants;
using TetrLoader.Enum;
using TetrLoader.JsonClass;
using TetrLoader.JsonClass.Event;

namespace TetrioVirtualEnvironment
{
	public partial class Environment
	{
		public const int FIELD_WIDTH = 10;
		public const int FIELD_HEIGHT = 40;
		public static int FIELD_SIZE => FIELD_WIDTH * FIELD_HEIGHT;

		public EventManager EventManager;

		public CustomStats CustomStats;
		public int GeneratedRngCount { get; private set; }
		private bool InfinityHold { get; } = false;
		public string? Username { get; }

		private List<int>? GarbageIDs { get; set; }

		public GameData GameData { get; private set; }
		public Stats Stats { get; private set; }

		/// <summary>
		/// RNG Generator
		/// </summary>
		public Rng Rng { get; internal set; }

		public NextGenerateKind NextGenerateMode { get; }
		public int NextSkipCount;


		/// <summary>
		/// for process events at same frame.
		/// </summary>
		public int CurrentIndex { get; private set; }

		public int CurrentFrame { get; private set; }
		public bool IsDead { get; internal set; } = false;


		/// <summary>
		/// event data of 'full'.use it for reset the game.
		/// full event is first event after game started existing in ttr/ttrm.
		/// </summary>
		internal EventFullData EventFull { get; }

		/// <summary>
		/// initialize game data.
		/// </summary>
		internal FieldData DataForInitialize { get; }

		/// <summary>
		/// ArrayMode only use initialized Next.
		/// SeedMode generate next from seed based on TETR.IO generate system.
		/// </summary>
		public enum NextGenerateKind : byte
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="envData"></param>
		/// <param name="envMode"></param>
		/// <param name="username"></param>
		/// <param name="dataForInitialize"></param>
		/// <param name="nextSkipCount"></param>
		public Environment(EventFullData envData, NextGenerateKind envMode, string? username, bool infinityHold = false,
			FieldData? dataForInitialize = null, int nextSkipCount = 0)
		{
			dataForInitialize ??= new FieldData();


			Username = username;
			NextGenerateMode = envMode;
			EventFull = envData;
			DataForInitialize = dataForInitialize;
			NextSkipCount = nextSkipCount;

			ResetGame(envData, envMode, dataForInitialize, nextSkipCount);
		}

		/// <summary>
		/// ResetGame for rewind
		/// </summary>
		/// <param name="envData"></param>
		/// <param name="envMode"></param>
		/// <param name="dataForInitialize"></param>
		/// <param name="nextSkipCount"></param>
		internal void ResetGame(EventFullData envData, NextGenerateKind envMode, FieldData dataForInitialize,
			int nextSkipCount = 0)
		{
			EventManager = new EventManager();
			GameData = new GameData();
			Stats = new Stats();
			CustomStats = new CustomStats();


			IsDead = false;
			GeneratedRngCount = 0;
			GarbageIDs = new List<int>();
			CurrentFrame = 0;
			CurrentIndex = 0;

			GameData.Init(envMode, envData, GameData, this, nextSkipCount, dataForInitialize);
		}


		public void KeyInput(EventType? type, EventKeyInputData @event)
		{
			if (type == EventType.Keydown)
			{
				if (@event.subframe > GameData.SubFrame)
				{
					ProcessShift(false, @event.subframe - GameData.SubFrame);
					FallEvent(null, @event.subframe - GameData.SubFrame);
					GameData.SubFrame = @event.subframe;
				}

				if (@event.key == "moveLeft")
				{
					GameData.LShift = true;
					GameData.LastShift = -1;
					GameData.LDas = @event.hoisted ? GameData.Handling.DAS - GameData.Handling.DCD : 0;
					if (GameData.Options.Version >= 12)
						GameData.LDasIter = GameData.Handling.ARR;

					ProcessLShift(true, GameData.Options.Version >= 15 ? 0 : 1);
					return;
				}

				if (@event.key == "moveRight")
				{
					GameData.RShift = true;
					GameData.LastShift = 1;
					GameData.RDas = @event.hoisted ? GameData.Handling.DAS - GameData.Handling.DCD : 0;
					if (GameData.Options.Version >= 12)
						GameData.RDasIter = GameData.Handling.ARR;

					ProcessRShift(true, GameData.Options.Version >= 15 ? 0 : 1);
					return;
				}

				if (@event.key == "softDrop")
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
					if (@event.key == "rotateCCW")
					{
						var e = GameData.Falling.R - 1;
						if (e < 0)
							e = 3;
						RotatePiece(e);
					}

					if (@event.key == "rotateCW")
					{
						var e = GameData.Falling.R + 1;
						if (e > 3)
							e = 0;
						RotatePiece(e);
					}

					if (@event.key == "rotate180" && GameData.Options.Allow180)
					{
						var e = GameData.Falling.R + 2;
						if (e > 3)
							e -= 4;
						RotatePiece(e);
					}

					if (@event.key == "hardDrop" && GameData.Options.AllowHardDrop &&
					    GameData.Falling.SafeLock == 0)
					{
						FallEvent(int.MaxValue, 1);
					}

					if (@event.key == "hold")
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
					ProcessShift(false, @event.subframe - GameData.SubFrame);
					FallEvent(null, @event.subframe - GameData.SubFrame);
					GameData.SubFrame = @event.subframe;
				}

				if (@event.key == "moveLeft")
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

				if (@event.key == "moveRight")
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

				if (@event.key == "softDrop")
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
			ProcessShift(false, 1 - GameData.SubFrame);
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
							GameData.Falling.Init(null, false, NextGenerateMode);
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
								Console.WriteLine("予期せぬEvent状態をしています。正常にリプレイが再生されない可能性があります。");


							if (GarbageIDs.Contains((int)garbageEvent.data.id))
								break;

							GarbageIDs.Add((int)garbageEvent.data.id);

							switch (garbageEvent.data.data.type)
							{
								case "attack":
									IncomingAttack(new GarbageData()
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
											StartingAttack(garbageEvent.data.data.data, garbageEvent.data.data.sender,
												idValue, garbageEvent.data.data.cid);

											break;
									}

									break;

								case "interaction_confirm":
									switch (garbageEvent.data.data.data.type)
									{
										case "garbage":
											string idValue;
											if (GameData.Options.ClipListenIDs)
												idValue = garbageEvent.data.data.sender_id != null
													? garbageEvent.data.data.sender_id.Substring(0, 24)
													: null;
											else
												idValue = garbageEvent.data.data.sender_id;

											IncomingAttack(garbageEvent.data.data.data, garbageEvent.data.data.sender,
												idValue, garbageEvent.data.data.cid);
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

		private void StartingAttack(GarbageData data, string sender, string? id1, int? id2)
		{
			InsertDamage(data, sender, id1, id2);
		}

		private void IncomingAttack(GarbageData data, string? sender = null, string? id1 = null, int? id2 = null)
		{
			if (GameData.Options.Passthrough == PassthroughType.Consistent)
			{
				data.amt = GetAmt(data, id1);
			}
			else
			{
				if (id2 != null)
					data.amt = GetImpendingGarbageLineAtID(id2);

				if (data.amt <= 0)
					return;
			}


			WaitFrames(GameData.Options.GarbageSpeed, "incoming-attack-hit", new IgeData()
			{
				data = data,
				sender = sender,
				cid = id2
			});


			int GetImpendingGarbageLineAtID(int? garbageID)
			{
				foreach (var garbage in GameData.ImpendingDamages)
				{
					if (garbage.cid == garbageID)
						return garbage.lines;
				}

				return 0;
			}
		}

		private void WaitFrames(int waitFrame, string kind, object data)
		{
			GameData.WaitingFrames.Add(new WaitingFrameData()
			{
				target = CurrentFrame + waitFrame,
				type = kind,
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
				IncomingAttackHit(igedata.data, igedata.sender, igedata.cid);
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


		private void IncomingAttackHit(GarbageData data, string sender, int? cid)
		{
			GameData.NotYetReceivedAttack--;

			if (cid != null)
			{
				//ActiveDamage Function
				foreach (var garbage in GameData.ImpendingDamages)
				{
					if (garbage.cid == cid)
						garbage.active = true;
				}
			}
			else
			{
				InsertDamage(data, sender);
			}
		}


		private void InsertDamage(GarbageData data, string sender, string? id1 = null, int? id2 = null)
		{
			var amt_value = data.amt;

			if (GameData.Options.Passthrough == PassthroughType.Zero)
			{
				amt_value = GetAmt(data, id1);
			}

			if (!(amt_value <= 0))
			{
				GameData.ImpendingDamages.Add(new IgeData()
				{
					id = ++GameData.GarbageID,
					type = data.type,
					active = id2 == null,
					lines = amt_value,
					column = data.column,
					data = data,
					sender = sender,
					cid = id2
				});
			}
		}

		private int GetAmt(GarbageData data, string? id = null)
		{
			GameData.GarbageActnowledgements.Incoming[id] = data.iid;

			var amtValue = data.amt;

			if (GameData.GarbageActnowledgements.Outgoing.ContainsKey(id))
			{
				var newArray = new List<GarbageData?>();

				foreach (var outgoing in GameData.GarbageActnowledgements.Outgoing[id])
				{
					if (outgoing.iid <= data.ackiid)
						continue;
					var minValue = Math.Min(outgoing.amt, amtValue);
					outgoing.amt -= minValue;
					amtValue -= minValue;


					if (outgoing.amt > 0)
						newArray.Add(outgoing);
				}

				GameData.GarbageActnowledgements.Outgoing[id] = newArray;
			}

			return amtValue;
		}


		private void FallEvent(int? value, double subFrameDiff)
		{
			if (GameData.Falling.SafeLock > 0)
				GameData.Falling.SafeLock--;

			if (GameData.Falling.Sleep || GameData.Falling.DeepSleep)
				return;

			var subframeGravity = GameData.Gravity * subFrameDiff;

			if (GameData.SoftDrop)
			{
				if (GameData.Handling.SDF == (GameData.Options.Version >= 15 ? 41 : 21))
					subframeGravity = (GameData.Options.Version >= 15 ? 400 : 20) * subFrameDiff;
				else
				{
					subframeGravity *= GameData.Handling.SDF;
					subframeGravity = Math.Max(subframeGravity,
						GameData.Options.Version >= 13 ? 0.05 * GameData.Handling.SDF : 0.42);
				}
			}

			if (value != null)
				subframeGravity = (int)value;

			if (!GameData.Options.InfiniteMovement &&
			    GameData.Falling.LockResets >= (int)GameData.Options.LockResets &&
			    !IsLegalAtPos((Tetrimino.MinoType)GameData.Falling.Type, GameData.Falling.X,
				    GameData.Falling.Y + 1, GameData.Falling.R, GameData.Board))
			{
				subframeGravity = 20;
				GameData.Falling.ForceLock = true;
			}


			if (!GameData.Options.InfiniteMovement &&
			    GameData.FallingRotations > (int)GameData.Options.LockResets + 15)
			{
				subframeGravity += 0.5 * subFrameDiff *
				                   (GameData.FallingRotations - ((int)GameData.Options.LockResets + 15));
			}

			double constSubFrameGravity = subframeGravity;

			for (; subframeGravity > 0;)
			{
				var ceiledValue = Math.Ceiling(GameData.Falling.Y);
				if (!HardDrop(Math.Min(1, subframeGravity), constSubFrameGravity))
				{
					if (value != null)
						GameData.Falling.ForceLock = true;
					Locking(value != 0 && value != null, subFrameDiff);
					break;
				}

				subframeGravity -= Math.Min(1, subframeGravity);
				if (ceiledValue != Math.Ceiling(GameData.Falling.Y))
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

			if (IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X, yPos1, GameData.Falling.R,
				    GameData.Board) &&
			    IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X, yPos2, GameData.Falling.R,
				    GameData.Board))
			{
				var highestY = GameData.Falling.HighestY;
				yPos2 = GameData.Falling.Y;

				GameData.Falling.Y = yPos1;
				GameData.Falling.HighestY = Math.Ceiling(Math.Max(GameData.Falling.HighestY, yPos1));
				GameData.Falling.Floored = false;
				if (Math.Ceiling(yPos1) != Math.Ceiling(yPos2))
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

			var isTspin = IsTspin();

			EventManager.Trigger_OnPiecePlaced(this, EventArgs.Empty);
			CustomStats.TotalPiecePlaced++;

			var clearedLineCount = ClearLines(out var garbageClear, out var stackClear);

			CustomStats.TotalGarbageClear += garbageClear;
			CustomStats.TotalStackClear += stackClear;

			var announceLine = GetAttackPower(clearedLineCount, isTspin);


			IsBoardEmpty();


			if (!announceLine)
				TakeAllDamage();


			GameData.Falling.Init(null, false, NextGenerateMode);
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
					if (IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X + 1,
						    GameData.Falling.Y, GameData.Falling.R, GameData.Board))
						GameData.Falling.X++;
					break;

				case Action.MoveLeft:
					if (IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X - 1,
						    GameData.Falling.Y, GameData.Falling.R, GameData.Board))
						GameData.Falling.X--;
					break;

				case Action.RotateLeft:
					var e = GameData.Falling.R - 1;
					if (e < 0)
						e = 3;
					RotatePiece(e);
					break;

				case Action.RotateRight:
					e = GameData.Falling.R + 1;
					if (e > 3)
						e = 0;
					RotatePiece(e);
					break;

				case Action.Rotate180:
					e = GameData.Falling.R + 2;
					if (e > 3)
						e -= 4;
					RotatePiece(e);
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
						if (IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X,
							    GameData.Falling.Y + 1, GameData.Falling.R, GameData.Board))
							GameData.Falling.Y++;
						else
							break;
					}

					break;

				case Action.MoveUp:
					if (IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X,
						    GameData.Falling.Y - 1, GameData.Falling.R, GameData.Board))
						GameData.Falling.Y--;
					break;

				case Action.MoveDown:
					if (IsLegalAtPos(GameData.Falling.Type, GameData.Falling.X,
						    GameData.Falling.Y + 1, GameData.Falling.R, GameData.Board))
						GameData.Falling.Y++;
					break;

				default: throw new Exception();
			}
		}

		/// <summary>
		/// This is for DEBUG
		/// </summary>
		/// <param name="kind"></param>
		/// <returns></returns>
		public object GetData(string kind)
		{
			switch (kind)
			{
				case "safelock":

					break;
			}

			if (kind == "safelock")
				return GameData.Falling.SafeLock;

			return null;
		}

		private int ClearLines(out int garbageClear, out int stackClear)
		{
			List<int> list = new List<int>();
			garbageClear = 0;
			stackClear = 0;

			for (int y = FIELD_HEIGHT - 1; y >= 0; y--)
			{
				int garbageCount = 0;

				bool flag = true;
				for (int x = 0; x < FIELD_WIDTH; x++)
				{
					if (GameData.Board[x + y * 10] == Tetrimino.MinoType.Empty)
					{
						flag = false;
						break;
					}
					else if (GameData.Board[x + y * 10] == Tetrimino.MinoType.Garbage)
						garbageCount++;
				}

				if (flag)
				{
					list.Add(y);

					if (garbageCount == FIELD_WIDTH - 1)
						garbageClear++;
					else
						stackClear++;
				}
			}

			list.Reverse();
			foreach (var value in list)
			{
				DownLine(value, GameData.Board);
			}

			return list.Count;
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
				if (IsLegalAtPos(type, x, y + testY,
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
		private void DownLine(int value, Tetrimino.MinoType[] field)
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
			GameData.Falling.Init(GameData.Hold, true, NextGenerateMode);
			GameData.Hold = tempCurrentType;
		}


		private void TakeAllDamage()
		{
			if (!GameData.Options.HasGarbage)
				return;

			var ABoolValue = true;
			int maxReceiveGarbage =
				false ? 400 : (int)Math.Min(GameData.Options.GarbageCapMax, GameData.Options.GarbageCap);
			var oValue = false;
			var iArray = new List<IgeData>();

			for (var impendingIndex = GameData.ImpendingDamages.Count - 1; impendingIndex >= 0; impendingIndex--)
			{
				if (!GameData.ImpendingDamages[impendingIndex].active)
				{
					iArray.Insert(0, GameData.ImpendingDamages[impendingIndex]);
					GameData.ImpendingDamages.RemoveAt(impendingIndex);
				}
			}

			for (var i = 0; i < maxReceiveGarbage && GameData.ImpendingDamages.Count != 0; i++)
			{
				GameData.ImpendingDamages[0].lines--;
				oValue = true;
				bool rValue;
				if (GameData.ImpendingDamages[0].lines <= 0)
					rValue = true;
				else
					rValue = i >= maxReceiveGarbage - 1;

				if (!PushGarbageLine(GameData.ImpendingDamages[0].column, false,
					    (ABoolValue ? 64 : 0) | (rValue ? 4 : 0)))
					break;

				ABoolValue = false;

				if (!(GameData.ImpendingDamages[0].lines > 0))
				{
					GameData.ImpendingDamages.RemoveAt(0);
					ABoolValue = true;
				}
			}

			GameData.ImpendingDamages.AddRange(iArray.ToArray());
		}
	}
}