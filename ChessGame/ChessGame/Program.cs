using System;
using System.Numerics;
using System.Text.RegularExpressions;

namespace ChessGame
{
	class Program
	{
		static void Main(string[] args)
		{
			var rx = new Regex(@"[A-H]{1}[1-8]{1}");

			ChessGame chessGame = new ChessGame();
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

	enum PieceColor
	{
		white,
		black
	}

	abstract class Piece
	{
		public readonly Piece[,] board;
		public readonly PieceColor color;

		public Piece(PieceColor _color, Piece[,] _board)
		{
			color = _color;
			board = _board;
		}

		public PieceColor GetColor()
		{
			return color;
		}

		public double GetSlope(int[] crds)
		{
			if(crds[0] - crds[2] == 0)
				return -1;
			else
				return Math.Abs(crds[1] - crds[3]) / Math.Abs(crds[0] - crds[2]);
		}

		public abstract char GetID();

		public abstract bool CalculateDestination(int[] coords);
	}

	class Rook : Piece
	{
		public Rook(PieceColor _color, Piece[,] _board) : base(_color, _board) { }

		public override bool CalculateDestination(int[] coords)
		{
			double slope = GetSlope(coords);

			if (coords[3] > coords[1]) //If the desired position is located in a lower row
			{
				if (coords[0] != coords[2]) //If the x axis is not the same between the current and desired positions
					return false;

				for (int i = coords[1] + 1; i <= coords[3]; i += 1) //Iterate between current position and desired position
				{
					if (board[i, coords[0]] != null) //If the current tile is not empty
					{
						//If the pieces are not of the same color and the target piece is on the same tile as the target position
						if (board[i, coords[0]].GetColor() != board[coords[1], coords[0]].GetColor() && i == coords[3])
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
			} //END OF PRIMARY IF STATEMENT//

			return true; //If no evaluations produce a return value, return true.(for debugging purposes)
		}

		public override char GetID()
		{
			return 'R';
		}
	}

	class Bishop : Piece
	{
		public Bishop(PieceColor _color, Piece[,] _board) : base(_color, _board) { }

		public override bool CalculateDestination(int[] coords)
		{
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
			} //END OF PRIMARY IF STATEMENT//

			return true;
		}

		public override char GetID()
		{
			return 'B';
		}
	}

	class Queen : Piece
	{
		public Queen(PieceColor _color, Piece[,] _board) : base(_color, _board) { }

		public override bool CalculateDestination(int[] coords)
		{
			double slope = GetSlope(coords);

			if(slope == -1) //if movement is vertical
			{

			}
			else if(slope == 0) //if movement is horizontal
			{

			}
			else if(slope == 1) //if movement is diagonal
			{

			}
			else
				return false;

			return true;
		}

		public override char GetID()
		{
			return 'Q';
		}
	}

	class King : Piece
	{
		public King(PieceColor _color, Piece[,] _board) : base(_color, _board) { }

		public override bool CalculateDestination(int[] coords)
		{
			return true;
		}

		public override char GetID()
		{
			return 'K';
		}
	}

	class Knight : Piece
	{
		public Knight(PieceColor _color, Piece[,] _board) : base(_color, _board) { }

		public override bool CalculateDestination(int[] coords)
		{
			return true;
		}

		public override char GetID()
		{
			return 'N';
		}
	}

	class Pawn : Piece
	{
		public Pawn(PieceColor _color, Piece[,] _board) : base(_color, _board) { }

		public override bool CalculateDestination(int[] coords)
		{
			return true;
		}

		public override char GetID()
		{
			return 'P';
		}
	}

	class ChessGame
	{

		// (|y - newY|)/(|x - newX|) = slope 
		//"y - newY" and "x - newX" can foresee directions based on
		//whether the resulting numbers are negative or positive

		//if slope == 1, movement is diagonal
		//if slope == 0, movement is horizontal
		//if x - newX == 0, movement is vertical

		//Flag for switching players
		//1 : player 1 : white
		//-1 : player 2 : black
		private int f_player = 1;
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
			 * The Rook can only move in rows and columns, and therefore
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
			board[7, 0] = new Rook(PieceColor.black, board);
			board[7, 1] = new Knight(PieceColor.black, board);
			board[7, 2] = new Bishop(PieceColor.black, board);
			board[7, 3] = new King(PieceColor.black, board);
			board[7, 4] = new Queen(PieceColor.black, board);
			board[7, 5] = new Bishop(PieceColor.black, board);
			board[7, 6] = new Knight(PieceColor.black, board);
			board[7, 7] = new Rook(PieceColor.black, board);

			board[0, 0] = new Rook(PieceColor.white, board);
			board[0, 1] = new Knight(PieceColor.white, board);
			board[0, 2] = new Bishop(PieceColor.white, board);
			board[0, 3] = new King(PieceColor.white, board);
			board[0, 4] = new Queen(PieceColor.white, board);
			board[0, 5] = new Bishop(PieceColor.white, board);
			board[0, 6] = new Knight(PieceColor.white, board);
			board[0, 7] = new Rook(PieceColor.white, board);

			for(int i = 0; i < 8; i += 1)
				board[1, i] = new Pawn(PieceColor.white, board);

			for(int i = 0; i < 8; i += 1)
				board[6, i] = new Pawn(PieceColor.black, board);

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
						Console.Write($" {board[i, j].GetID()} |");
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
		*
		* 
		* coords[0] : Piece's current index position on the x axis.
		* coords[1] : Piece's current index position on the y axis.
		* coords[2] : Piece's desired index position on the x axis.
		* coords[3] : Piece's desired index position on the y axis.
		*/

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
			y = int.Parse(pieceToMove[1].ToString()) - 1;

			newX = ParseLetter(newLocation);
			newY = int.Parse(newLocation[1].ToString()) - 1;

			if (board[y, x] == null || (x == newX && y == newY)) //Return false if a piece is present, or the desired position matches the current.
				return false;

			if (board[y, x].CalculateDestination(new int[] { x, y, newX, newY }))
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