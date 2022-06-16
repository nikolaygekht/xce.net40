/**
 * The line vector contains information about each of the lines in a cell buffer.
 */
class LineVector {
    Partitioning starts;
    int markers[16];
public:

    LineVector();
    ~LineVector();
    void Init();

    void InsertText(int line, int delta);
    void InsertLine(int line, int position, bool lineStart);
    void SetLineStart(int line, int position);
    void RemoveLine(int line);

    int Lines() const {
        return starts.Partitions();
    }
    int LineFromPosition(int pos) const;
    int LineStart(int line) const {
        return starts.PositionFromPartition(line);
    }

    int GetMarker(int marker);
    void SetMarker(int marker, int line);
};

