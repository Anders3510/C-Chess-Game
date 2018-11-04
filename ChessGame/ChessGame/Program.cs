﻿using System;
using System.Text.RegularExpressions;

namespace ChessGame
{
	class Program
	{
		static void Main(string[] args)
		{
			var rx = new Regex(@"[A-H]{1}[1-8]{1}");

			ChessGame chessGame = new ChessGame();
			chessGame.f_player = 1;
			bool isRunning = true;
			string input;

			string pieceToMove;
			string newLocation;

			while (isRunning)
			{
				//Print gameboard
				chessGame.PrintBoard();

				//Prompt user and check input
				Console.WriteLine("What piece would you like to move?");
				input = Console.ReadLine();
				if (!rx.IsMatch(input))
				{
					Console.Clear();
					Console.WriteLine("Wrong input format.");
					continue;
				}
				pieceToMove = input;

				//Prompt user and check input
				Console.WriteLine("Where would you like to move it?");
				input = Console.ReadLine();
				while (!rx.IsMatch(input))
				{
					Console.Clear();
					Console.WriteLine("Wrong input format.");
					input = Console.ReadLine();
				}
				newLocation = input;

				if (chessGame.MovePiece(pieceToMove, newLocation))
					Console.WriteLine("Piece moved successfully!");
				else
					Console.WriteLine("Piece could not be moved.");
			}
			Console.ReadLine();
		}
	}

	class ChessGame
	{
		public enum PieceType
		{
			Pawn,
			Rook,
			Knight,
			Bishop,
			Queen,
			King
		}

		public class Piece
		{
			//Color is represented by an integer
			//1 : white
			//2 : black
			public int color;
			public char id;
			public PieceType type;

			public Piece(int _color, PieceType _type)
			{
				color = _color;
				type = _type;
				switch (type)
				{
					case PieceType.Pawn:
						id = 'P';
						break;

					case PieceType.Rook:
						id = 'R';
						break;

					case PieceType.Knight:
						id = 'N';
						break;

					case PieceType.Bishop:
						id = 'B';
						break;

					case PieceType.Queen:
						id = 'Q';
						break;

					case PieceType.King:
						id = 'K';
						break;
				}
			}
		}

		//Flag for switching players
		//1 : player 1 : white
		//-1 : player 2 : black
		public int f_player = 1;
		//Gameboard
		public Piece[,] board = new Piece[8, 8];

		public ChessGame()
		{
			/*
			 * 1  R N B K Q B N R
			 * 2  P P P P P P P P
			 * 3
			 * 4
			 * 5
			 * 6
			 * 7  P P P P P P P P
			 * 8  R N B K Q B N R
			 *    A B C D E F G H
			 * 
			 * {
			 * {0,0 0,1 0,2 0,3 0,4 0,5 0,6 0,7}
			 * {1,0 1,1 1,2 1,3 1,4 1,5 1,6 1,7}
			 * {2,0 2,1 2,2 2,3 2,4 2,5 2,6 2,7}
			 * {3,0 3,1 3,2 3,3 3,4 3,5 3,6 3,7}
			 * {4,0 4,1 4,2 4,3 4,4 4,5 4,6 4,7}
			 * {5,0 5,1 5,2 5,3 5,4 5,5 5,6 5,7}
			 * {6,0 6,1 6,2 6,3 6,4 6,5 6,6 6,7}
			 * {7,0 7,1 7,2 7,3 7,4 7,5 7,6 7,7} 
			 * }
			 * 
			 * Movement patterns:
			 * Rook:
			 * The Rook can only move in rows and column, and therefore
			 * can either move to the same index in a lower or higher indexed array contained in the first dimension,
			 * or it can move to any other position within its current second dimension.
			 * 
			 * Bishop:
			 * The Bishop can only move diagonally and therefore its second-dimensional index must be shifted by 1
			 * in either direction for every step it takes in the first dimension.
			 * 
			 * Queen:
			 * The Queen possesses the movement capabilities of both the Rook and the Bishop.
			 * 
			 * King:
			 * The King may only move 1 tile at a time, and therefore its indexed position
			 * can only be shifted by 1 in any direction, in both the first and the second dimension.
			 * 
			 * Pawn:
			 * The Pawn can only move forward in a straight line,
			 * except when there is a piece of the opposite color on a tile that is located 
			 * on either of the 2 frontal diagonal places.
			 * The pawn may move 2 steps straight forward on its very first move.
			 * If the pawn reaches the opposite side of the board, it may be converted into
			 * a Rook, Bishop, Knight, or a Queen.
			 * 
			 * Knight:
			 * The Knight may move to any position that is 2 steps in any of the 4 cardinal directions,
			 * and then displaced 1 step to either the right or left.
			 * Unlike all other pieces, the Knight is not hindered by other pieces being in its way.
			 * 
			 */
			//Add pieces to board and set appropriate types and colors
			//Assign an array of possible places to which the piece can move
			//when it is instatiated, using the constructor function
			int counter = 0;
			for(int i = 0; i < 2; i += 1)
			{
				board[counter, 0] = new Piece(2, PieceType.Rook);
				board[counter, 1] = new Piece(2, PieceType.Knight);
				board[counter, 2] = new Piece(2, PieceType.Bishop);
				board[counter, 3] = new Piece(2, PieceType.King);
				board[counter, 4] = new Piece(2, PieceType.Queen);
				board[counter, 5] = new Piece(2, PieceType.Bishop);
				board[counter, 6] = new Piece(2, PieceType.Knight);
				board[counter, 7] = new Piece(2, PieceType.Rook);
				counter += 7;
			}

			int k = 0;
			while (k < 8)
			{
				board[1, k] = new Piece(2, PieceType.Pawn);
				k += 1;
			}
			k = 0;
			while (k < 8)
			{
				board[6, k] = new Piece(1, PieceType.Pawn);
				k += 1;
			}
		}

		public void PrintBoard()
		{
			Console.WriteLine(" _______________________________");
			//Iterate Rows
			for (int i = 0; i < board.GetLength(0); i += 1)
			{
				Console.Write("|");
				//Iterate Columns
				for (int j = 0; j < board.GetLength(1); j += 1)
				{
					if (board[i, j] == null)
						Console.Write("   |");
					else
						Console.Write($" {board[i, j].id} |");
				}
				Console.Write("\n|_______________________________|\n");
			}
		}
		/*
		* Object at position [newY,newX] must be null,
		* unless the object is of the opposite color,
		* then the piece may move to that position,
		* but in the case of Rooks, Bishops, and similar,
		* the piece must not be allowed to move further along their path.
		* A Piece's path must also be blocked by Pieces of its color, 
	 	* but it must not be allowed to take their place.
		* 2,6
		* 
		* coords[0] : Piece's current index position on the x axis.
		* coords[1] : Piece's current index position on the y axis.
		* coords[2] : Piece's desired index position on the x axis.
		* coords[3] : Piece's desired index position on the y axis.
		*/
		private bool CalculateDestination(int[] coords, PieceType type)
		{
			if (coords[0] == coords[2] && coords[1] == coords[3]) //If the destination coordinates match the current position
				return false;

			switch (type)
			{
				case PieceType.Rook:
					if (coords[3] > coords[1]) //If the desired position is located in a lower row
					{
						if (coords[0] != coords[2]) //If the x axis is not the same between the current and desired positions
							return false;

						for (int i = coords[1] + 1; i <= coords[3]; i += 1) //Iterate between current position and desired position
						{
							if (board[i, coords[0]] != null) //If the current tile is not empty
							{
								//If the pieces are not of the same color and the target piece is on the same tile as the target position
								if (board[i, coords[0]].color != board[coords[1], coords[0]].color && i == coords[3])
								{
									return true;
								}
								else //The pieces are of the same color, or the desired position is not the same as the current piece's position
								{
									return false;
								}
							}
						}
					}
					else if (coords[3] < coords[1]) //If the desired position is located in a higher row
					{
						if (coords[0] != coords[2]) //If the x axis is not the same between the current and desired positions
							return false;

						for (int i = coords[1] - 1; i >= coords[3]; i -= 1) //Iterate between current position and desired position
						{
							if (board[i, coords[0]] != null) //If the current tile is not empty
							{
								//If the pieces are not of the same color and the target piece is on the same tile as the target position
								if (board[i, coords[0]].color != board[coords[1], coords[0]].color && i == coords[3])
								{
									return true;
								}
								else //The pieces are of the same color, or the desired position is not the same as the current piece's position
								{
									return false;
								}
							}
						}
					}
					else //In this case, the desired position is located on the same row, in a different column
					{
						if (coords[2] > coords[0]) //The desired horizontal position is located right of the current position
						{
							for (int i = coords[0] + 1; i <= coords[2]; i += 1)
							{
								if (board[coords[1], i] != null) //If the current tile is not empty
								{
									//If the pieces are not of the same color and the target piece is on the same tile as the target position
									if (board[coords[1], i].color != board[coords[1], coords[0]].color && i == coords[3])
									{
										return true;
									}
									else //The pieces are of the same color, or the desired position is not the same as the current piece's position
									{
										return false;
									}
								}
							}
						}
						else //The desired horizontal position is located left of the current position
						{
							for (int i = coords[0] - 1; i >= coords[2]; i -= 1)
							{
								if (board[coords[1], i] != null) //If the current tile is not empty
								{
									//If the pieces are not of the same color and the target piece is on the same tile as the target position
									if (board[coords[1], i].color != board[coords[1], coords[0]].color && i == coords[3])
									{
										return true;
									}
									else //The pieces are of the same color, or the desired position is not the same as the current piece's position
									{
										return false;
									}
								}
							}
						}
					}
					break;

				/*
				 * -y, -x			-y, +x
				 * 
				 * +y, -x			+y, +x
				 */
				case PieceType.Bishop:
					int j;
					if (coords[0] == coords[2] || coords[1] == coords[3])
						return false;

					if (coords[0] < coords[2] && coords[1] > coords[3]) //Piece's destination is towards the top right
					{
						j = coords[0] + 1;
						for (int i = coords[1] - 1; i >= coords[3]; i -= 1)
						{
							if (board[i, j] != null)
							{
								//If the pieces are not of the same color and the target piece is on the same tile as the target position
								if (board[i, j].color != board[coords[1], coords[0]].color && (j == coords[2] && i == coords[3]))
								{
									return true;
								}
								else //The pieces are of the same color, or the desired position is not the same as the current piece's position
								{
									return false;
								}
							}
							j += 1;
						}
					}
					else if (coords[0] < coords[2] && coords[1] < coords[3]) //Piece's destination is towards the bottom right
					{
						j = coords[0] + 1;
						for (int i = coords[1] + 1; i <= coords[3]; i += 1)
						{
							if (board[i, j] != null)
							{
								//If the pieces are not of the same color and the target piece is on the same tile as the target position
								if (board[i, j].color != board[coords[1], coords[0]].color && (j == coords[2] && i == coords[3]))
								{
									return true;
								}
								else //The pieces are of the same color, or the desired position is not the same as the current piece's position
								{
									return false;
								}
							}
							j += 1;
						}
					}
					else if (coords[0] > coords[2] && coords[1] > coords[3]) //Piece's destination is towards the top left
					{
						j = coords[0] - 1;
						for (int i = coords[1] - 1; i >= coords[3]; i -= 1)
						{
							if (board[i, j] != null)
							{
								//If the pieces are not of the same color and the target piece is on the same tile as the target position
								if (board[i, j].color != board[coords[1], coords[0]].color && (j == coords[2] && i == coords[3]))
								{
									return true;
								}
								else //The pieces are of the same color, or the desired position is not the same as the current piece's position
								{
									return false;
								}
							}
							j -= 1;
						}
					}
					else if (coords[0] > coords[2] && coords[1] < coords[3]) //Piece's destination is towards the bottom left
					{
						j = coords[0] - 1;
						for (int i = coords[1] + 1; i <= coords[3]; i += 1)
						{
							if (board[i, j] != null)
							{
								//If the pieces are not of the same color and the target piece is on the same tile as the target position
								if (board[i, j].color != board[coords[1], coords[0]].color && (j == coords[2] && i == coords[3]))
								{
									return true;
								}
								else //The pieces are of the same color, or the desired position is not the same as the current piece's position
								{
									return false;
								}
							}
							j -= 1;
						}
					}
					break;
			}
			return true;
		}

		private int ParseLetter(string input)
		{
			int x;
			switch (input[0])
			{
				case 'A': x = 0; break;

				case 'B': x = 1; break;

				case 'C': x = 2; break;

				case 'D': x = 3; break;

				case 'E': x = 4; break;

				case 'F': x = 5; break;

				case 'G': x = 6; break;

				case 'H': x = 7; break;

				default: x = -1; break;
			}
			return x;
		}

		public bool MovePiece(string pieceToMove, string newLocation)
		{
			//Declarations
			int x;
			int y;
			int newX;
			int newY;

			//Parse strings into coordinates and run checks
			x = ParseLetter(pieceToMove);
			y = Int32.Parse(pieceToMove[1].ToString()) - 1;
			if (board[y, x] == null)
				return false;

			newX = ParseLetter(newLocation);
			newY = Int32.Parse(newLocation[1].ToString()) - 1;
			if (CalculateDestination(new int[] { x, y, newX, newY }, board[y, x].type))
			{
				//Move the piece
				board[newY, newX] = board[y, x];
				board[y, x] = null;
				//Change player
				f_player *= -1;
				return true;
			}
			else
				return false;
		}
	}
}