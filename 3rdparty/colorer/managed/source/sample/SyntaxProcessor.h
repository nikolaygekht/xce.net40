#ifndef __SyntaxProcessorH
#define __SyntaxProcessorH

#include "vectors/vectors.h"
#include "colorer\parserFactory.h"
#include "colorer\Editor\BaseEditor.h"
#include "CRegExp\CRegExp.h"
#include "IEditor.h"

class CColoredFile;                     //one file, which should processed by colorer
class CColoringInfo;                    //information about coloring
class CCheckPairInfo;                   //information about pairs of logical structures
class CRegionInfo;                      //information about coloring region
class CXBuilder;

/** The main facade class for syntax colorer. */
class CSyntaxColorer
{
 public:
    /** Constructor. */
    CSyntaxColorer(CXBuilder &rBuilder);

    /** Destructor. */
    ~CSyntaxColorer();

    /** Initialize the colorer.

        @param szColorerCatalogName     File name of the colorer's catalog file.
        @param szHrdClass               Region class
        @param szHrdName                Region name
        @param iBackParse               Back parse
        @param iMaxLen                  max length string for parsing
      */
    bool init(const char *szColorerCatalogName,
              const char *szHrdClass,
              const char *szHrdName,
              int iBackParse,
              int iMaxLen);

    /** Free the colorer initialization. */
    void free();

    /** Create colorer for specified file.

        @param pFile        The XCE's file to be colored.
      */
    CColoredFile *createColorer(CEditorFile *pFile);

    /** Get color of region
        @param szRegion         name of syntax region
        @param pColor           if region exist in colorer hrd scheme
                                then return color assign with this region
                                else return default color of scheme(def:text)

        @return                 true if region exist
    */

    bool getColorOfRegion(const char *szRegion, unsigned short *pColor);

 protected:
    ParserFactory *m_pFactory;               //!< the colorer's parser's factory class.
    StyledHRDMapper *m_pRegionMapper;        //!< current colorer's region mapper
    int m_iBackParse;                        //!< back parse
    int m_iMaxLen;                           //!< Max Length string for parsing
    CXBuilder &m_rBuilder;                   //!< XCE class factory
    const StyledRegion *m_pDefaultRegion;    //!< default region (def::text)
};

/** The instance of coloring parser for one XCE's file. */
class CColoredFile
{
    friend class CSyntaxColorer;
 protected:
    /** Constructor. */
    CColoredFile(ParserFactory *pParser,
                 CEditorFile *pFile,
                 StyledHRDMapper *pRegionMapper,
                 int iBackParse,
                 CXBuilder &rBuilder);

 public:
    /** Destructor.

        Note that all instances of CColoredFile should
        be destructed before instance of CSyntaxColorer.
      */
    ~CColoredFile();

    /** Prepare colorer for displaying (or other kind of processing)
        of specified area.

        @param iFirstLine       First line to be displayed.
        @param iLineNumber      Number of lines to be displayed.
      */
    bool prepareColoring(int iFirstLine, int iLineNumber);

    /** Notify colorer that line was changed.

        @param iLine            Number of line, which was changed.
      */
    bool notifyAboutLineChange(int iLine);

    /** Notify colorer that file name was changed. */
    bool notifyAboutFileNameChange();

    /** Notify colorer that entire document was seriously changed.

        @param iFirstLine       First line changed
      */
    bool notifyAboutMajorChange(int iFirstLine);

    /** Start enumeration of coloring information for
        specified line.

        Returns true if coloring information is available
        for this line.

        @param iLine            The line number for receiving coloring
                                information
        @param iPos             The position in line
      */
    bool startColorInfoEnumeration(int iLine, int iPos = 0);

    /** Get next part of coloring information.

        Returns null if no more coloring information
        is available
      */
    CColoringInfo *getColorInformation();

    /** Match pair for specified position local screen.

        @param iLine        Line in the file
        @param iPos         Position in the line
        @return             Instance of CCheckPairInfo object
                            or null is there is no match
      */
    CCheckPairInfo *checkLocalPair(int iLine, int iPos);

    /** Match pair for specified position global in all file.

        @param iLine        Line in the file
        @param iPos         Position in the line
        @return             Instance of CCheckPairInfo object
                            or null is there is no match
      */
    CCheckPairInfo *checkGlobalPair(int iLine, int iPos);

    /** Get name of the file type.

        The method can return empty string if type
        is unknown.
      */
    const char *getFileType();

    /** Need spelling by default with this kind type file or not.
    */
    bool needSpelling() const;

    /** Idle job. */
    void onIdle();

 protected:
    /** The adapter is used to give colorer
        access to XCE's files. */
    class CColorerLineSourceAdapter : public LineSource
    {
     public:
        /** Constructor.

            @param pFile        The XCE's file.
          */
        CColorerLineSourceAdapter(CEditorFile *pFile);

        /** Destructor. */
        ~CColorerLineSourceAdapter();

        /** Overridable: Job started.

            @param lno      First line number.
          */
        virtual void startJob(int lno);

        /** Overridable: Job finished.

            @param lno      Last line number.
          */
        virtual void endJob(int lno);

        /** Overridable: Get contents of specified string

            @param lno      Line number to get

            @return         String object, which should be valid
                            until next call of getLine() or
                            endJob()
          */
        virtual String *getLine(int lno);
      protected:
        DString *m_pLastString;                     //!< latest returned string
        CEditorFile *m_pFile;                       //!< reference to XCE's file.
        static long m_lMaxLen;                      //!< Max len of string for parsing
    };

    ParserFactory *m_pParser;                       //!< the colorer's parser's factory class.
    CColorerLineSourceAdapter *m_pAdapter;          //!< adapter between XCE's file and colorer
                                                    //!< input interface
    BaseEditor *m_pEditor;                          //!< the colorer's helper object for editor
    LineRegion *m_pLastRegion;                      //!< latest file region
    CEditorFile *m_pFile;                           //!< related editor file
    CColoringInfo *m_pColoringInfo;                 //!< cashing wraper on Line region
    bool m_bSpelling;                               //!< need spelling
    TPtrVector<string*> m_aSpellingRegions;         //!< spelling region
    bool m_bDefaultSpelling;                        //!< default spelling
};

/** Description of one coloring region. */
class CColoringInfo : public ISyntaxInfo
{
    friend class CColoredFile;

 protected:
    /** Constructor.

        @param pInfo        Colorer's region information.
      */
    CColoringInfo(TPtrVector<string*> &aSpellingRegions, LineRegion *pInfo);
    /* for improved perfomance of */
    CColoringInfo(TPtrVector<string*> &aSpellingRegions)
    : m_aSpellingRegions(aSpellingRegions)
    {};
    /*  Set line region pointer */
    int setColoringInfo(LineRegion *pInfo);

 public:
    /** Get start of region. */
    int getStartPos() const;

    /** Get length of region. */
    int getLength() const;

    /** Is the block a part of syntax region?. */
    bool isSyntaxRegion() const;

    /** Return colored reggion or no*/
    bool isColored() const;

    /** Get color of region*/
    unsigned short getColor() const;

    /** Get full name of syntax region.

        The method can return empty string
        if the block is not a syntax region.
      */
    const char *getRegionName() const;

    /** Get base name of syntax region.

        The method can return empty string
        if the block is not a syntax region.
      */
    const char *getBaseRegionName() const;
    /** This regions speling
    */
    bool isSpellingRegion() const;
 public:
     static unsigned short s_uBackGroundColor;       //!< default background color
     static unsigned short s_uForeGroundColor;       //!< default foreground color
 protected:
    LineRegion *m_pInfo;                            //!< line region.
    TPtrVector<string*> &m_aSpellingRegions;                  //!< spelling region
};

/** The information about other side of the pair (for example - brackets). */
class CCheckPairInfo : public ICheckPairInfo
{
    friend class CColoredFile;

 protected:
    /** Constructor.

        @param pInfo        Colorer's region information.
      */
    CCheckPairInfo(PairMatch *pInfo);
 public:

    /** Get line where is start of the pair*/
    int getStartLine() const;

    /** Get column where is start of the pair*/
    int getStartPos() const;

    /** Get length of pair start*/
    int getStartLength() const;

    /** Get line where is end of the pair*/
    int getEndLine() const;

    /** Get column where is end of the pair*/
    int getEndPos() const;

    /** Get length of pair end*/
    int getEndLength() const;

    /** Check is specified position at start of
        the pair.

        @param iLine        The line to be checked
        @param iPos         The columns the be checked
      */
    bool isStartOfPair(int iLine, int iPos);

    /** Check is specified position at end of
        the pair.

        @param iLine        The line to be checked
        @param iPos         The columns the be checked
      */
    bool isEndOfPair(int iLine, int iPos);

    /** Release object. */
    virtual void release();

 protected:
    int m_iStartLine;           //!< line where is start of the pair
    int m_iStartPos;            //!< column where is start of the pair
    int m_iStartLength;         //!< length of pair start
    int m_iEndLine;             //!< line where is end of the pair
    int m_iEndPos;              //!< column where is end of the pair
    int m_iEndLength;           //!< length of pair end
};
#endif
