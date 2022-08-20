using System.Collections.Generic;

[System.Serializable]
public class PieceCombo
{
    List<Piece> piece_list = new List<Piece>();
	public List<Piece> GetList() { return piece_list; }
	public int GetListCount() { return piece_list.Count; }

	public PieceCombo() {}
	
	public PieceCombo(List<Piece> piece_list) {
		this.piece_list = piece_list;
	}	
}
