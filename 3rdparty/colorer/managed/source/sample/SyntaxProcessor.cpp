#include "stdhead.h"
#include "colorer\parserFactory.h"
#include "colorer\Editor\BaseEditor.h"
#include "CRegExp\CRegExp.h"
#include "CEditorFile.h"
#include "XBuilder.h"

#include "SyntaxProcessor.h"



unsigned short CColoringInfo::s_uBackGroundColor = 0x70;
unsigned short CColoringInfo::s_uForeGroundColor = 0x00;
long CColoredFile::CColorerLineSourceAdapter::m_lMaxLen = 5000;
/** Constructor. */
CSyntaxColorer::CSyntaxColorer(CXBuilder &rBuilder) :
m_rBuilder(rBuilder)
{
    m_pFactory = 0;
}

/** Destructor. */
CSyntaxColorer::~CSyntaxColorer()
{
    free();
}

/** Initialize the colorer.

    @param szColorerCatalogName     File name of the colorer's catalog file.
    @param szHrdClass               Region class
    @param szHrdName                Region name
    @param iBackParse               Back parse
    @param iMaxLen                  max length string for parsing
*/
bool CSyntaxColorer::init(const char *szColorerCatalogName,
                          const char *szHrdClass,
                          const char *szHrdName,
                          int iBackParse,
                          int iMaxLen)
{
    try
    {
        SString sPath(const_cast<char *>(szColorerCatalogName));
        m_pFactory = new ParserFactory(&sPath);

        m_pRegionMapper = m_pFactory->createStyledMapper(&DString(szHrdClass), &DString(szHrdName));
        if (!m_pRegionMapper)
        {
            m_pRegionMapper = m_pFactory->createStyledMapper(&DString("console"), &DString("default"));
        }

        m_pDefaultRegion = StyledRegion::cast(m_pRegionMapper->getRegionDefine(DString("def:Text")));

        CColoringInfo::s_uBackGroundColor = m_pDefaultRegion->back ;
        CColoringInfo::s_uForeGroundColor = m_pDefaultRegion->fore;
        m_iBackParse = iBackParse;
        m_iMaxLen = iMaxLen;

    }
    catch (...)
    {
        return false;
    }
    return true;

}

/** Free the colorer initialization. */
void CSyntaxColorer::free()
{
    if (m_pFactory)
        delete m_pFactory;
    m_pFactory = null;
}

/** Get color of region
    @param szRegion         name of syntax region
    @param pColor           if region exist in colorer hrd scheme
                            then return color assign with this region
                            else return default color of scheme(def:text)

    @return                 true if region exist
*/

bool CSyntaxColorer::getColorOfRegion(const char *szRegion, unsigned short *pColor)
{
    //if (m_pInfo->rdef != 0 && m_pInfo->special == 0)
    const StyledRegion *pInfo = StyledRegion::cast(m_pRegionMapper->getRegionDefine(DString(szRegion)));
    if (!pInfo)
    {
        *pColor = (unsigned short)(m_pDefaultRegion->fore + (m_pDefaultRegion->back<<4));
        return false;
    }
    // from color map
    *pColor = (unsigned short)(pInfo->fore + (pInfo->back<<4));
    // if not defined used default editor color
    if (!pInfo->bfore)
        (*pColor) = (*pColor & 0xF0) + (m_pDefaultRegion->fore & 0xF);
    if (!pInfo->bback)
        *pColor = (*pColor & 0xF) + (m_pDefaultRegion->back << 4/* 0xF0*/);
    return true;;

}

/** Create colorer for specified file.

    @param pFile        The XCE's file to be colored.
  */
CColoredFile *CSyntaxColorer::createColorer(CEditorFile *pFile)
{
    return new CColoredFile(m_pFactory, pFile, m_pRegionMapper, m_iBackParse, m_rBuilder);
}

/** Constructor.

    @param pFile        The XCE's file.
  */
CColoredFile::CColorerLineSourceAdapter::CColorerLineSourceAdapter(CEditorFile *pFile)
{
    m_pFile = pFile;
    m_pLastString = 0;
}

/** Destructor. */
CColoredFile::CColorerLineSourceAdapter::~CColorerLineSourceAdapter()
{
    endJob(0);
}

/** Overridable: Job started.

    @param lno      First line number.
  */
void CColoredFile::CColorerLineSourceAdapter::startJob(int lno)
{
}

/** Overridable: Job finished.

    @param lno      Last line number.
  */
void CColoredFile::CColorerLineSourceAdapter::endJob(int lno)
{
    if (m_pLastString)
        delete m_pLastString;
    m_pLastString = 0;

}

/** Overridable: Get contents of specified string

    @param lno      Line number to get

    @return         String object, which should be valid
                    until next call of getLine() or
                    endJob()
  */
String *CColoredFile::CColorerLineSourceAdapter::getLine(int lno)
{
    long lLen;

    if (m_pLastString)
        delete m_pLastString;

    if (lno < 0 || lno >= m_pFile->getLinesCount())
    {
        m_pLastString = new DString("", 0, 1);
        return m_pLastString;
    }
    else
    {

        lLen = m_pFile->getLineLen(lno);
        if (lLen > m_lMaxLen && m_lMaxLen > 0)
            lLen = m_lMaxLen;


        if (m_pFile->getLine(lno) != 0)
            m_pLastString = new DString(m_pFile->getLine(lno), 0, lLen);
        else
            m_pLastString = new DString("", 0, 1);

        return m_pLastString;
    }
}

/** Constructor. */
CColoredFile::CColoredFile(ParserFactory *pParser,
                           CEditorFile *pFile,
                           StyledHRDMapper *pRegionMapper,
                           int iBackParse,
                           CXBuilder &rBuilder)
{
    m_pParser = pParser;
    m_pFile = pFile;
    m_pAdapter = new CColorerLineSourceAdapter(pFile);
    m_pEditor = 0;
    m_bSpelling = 0;
    m_pColoringInfo = NULL;
    if (m_pParser)
    {
        try
        {
            m_pEditor = new BaseEditor(m_pParser, m_pAdapter);
            SString sName(const_cast<char *>(m_pFile->getFullFileName()));
            m_pEditor->chooseFileType(&sName);
            m_pEditor->setRegionCompact(true);
            m_pEditor->setRegionMapper(pRegionMapper);
            m_pEditor->setBackParse(iBackParse);
            m_pEditor->lineCountEvent(m_pFile->getLinesCount());

            rBuilder.getSpellingRegions(m_pFile->getFullFileName(), m_aSpellingRegions);

            m_pColoringInfo = new CColoringInfo(m_aSpellingRegions);
        }
        catch (...)
        {
            if (m_pEditor)
                delete m_pEditor;
            m_pEditor = 0;
        }
    }
}

/** Destructor.

    Note that all instances of CColoredFile should
    be destructed before instance of CSyntaxColorer.
  */
CColoredFile::~CColoredFile()
{
    delete m_pAdapter;
    if (m_pEditor)
        delete m_pEditor;
    if (m_pColoringInfo)
        delete m_pColoringInfo;
}

/** Need spelling with this kind type file or not.
    */
bool CColoredFile::needSpelling() const
{
    return m_aSpellingRegions.size() > 0;
}
/** Prepare colorer for displaying (or other kind of processing)
    of specified area.

    @param iFirstLine       First line to be displayed.
    @param iLineNumber      Number of lines to be displayed.
  */
bool CColoredFile::prepareColoring(int iFirstLine, int iLineNumber)
{
    try
    {
        if (m_pEditor)
        {
            m_pEditor->lineCountEvent(m_pFile->getLinesCount());
            m_pEditor->visibleTextEvent(iFirstLine, iLineNumber);
            return true;
        }
        else
            return false;
    }
    catch(...)
    {
        return false;
    }
}

/** Notify colorer that line was changed.

    @param iLine            Number of line, which was changed.
  */
bool CColoredFile::notifyAboutLineChange(int iLine)
{
    try
    {
        if (m_pEditor && iLine >= 0 && iLine < m_pFile->getLinesCount())
        {
            m_pEditor->modifyLineEvent(iLine);
            m_pEditor->lineCountEvent(m_pFile->getLinesCount());
            return true;
        }
        else
            return false;
    }
    catch(...)
    {
        return false;
    }
}

/** Notify colorer that entire document was seriously changed.

    @param iFirstLine       First line changed
  */
bool CColoredFile::notifyAboutMajorChange(int iFirstLine)
{
    try
    {
        if (m_pEditor && iFirstLine >= 0 && iFirstLine < m_pFile->getLinesCount())
        {
            m_pEditor->modifyEvent(iFirstLine);
            m_pEditor->lineCountEvent(m_pFile->getLinesCount());
            return true;
        }
        else
            return false;
    }
    catch(...)
    {
        return false;
    }
}

/** Notify colorer that file name was changed. */
bool CColoredFile::notifyAboutFileNameChange()
{
    try
    {
        if (m_pEditor)
        {
            SString sName(const_cast<char *>(m_pFile->getFileName()));
            m_pEditor->chooseFileType(&sName);
            return true;
        }
        else
            return false;
    }
    catch(...)
    {
        return false;
    }
}


/** Start enumeration of coloring information for
    specified line.

    Returns true if coloring information is available
    for this line.

    @param iLine            The line number for receiving coloring
                            information
  */
bool CColoredFile::startColorInfoEnumeration(int iLine, int iPos)
{
    if (!m_pEditor || !(iLine >= 0 && iLine < m_pFile->getLinesCount()))
    {
        m_pLastRegion = 0;
        return false;
    }
    try
    {
        m_pLastRegion = m_pEditor->getLineRegions(iLine);

        //ng: correct end processing...
        while (true)
        {
            if (m_pLastRegion == 0)
                break;
            if (m_pLastRegion->end == -1)
                break;
            if (m_pLastRegion->end > iPos)
                break;

            m_pLastRegion = m_pLastRegion->next;
        }

        return m_pLastRegion != 0;
    }
    catch(...)
    {
        return false;
    }

}

/** Get next part of coloring information.

    Returns null if no more coloring information
    is available
  */
CColoringInfo *CColoredFile::getColorInformation()
{
    if (m_pLastRegion)
    {
        LineRegion *pReg = m_pLastRegion;
        m_pLastRegion = m_pLastRegion->next;
        m_pColoringInfo->setColoringInfo(pReg);
        return m_pColoringInfo;
    }
    else
        return 0;
}


/** Get name of the file type.

    The method can return empty string if type
    is unknown.
  */
const char *CColoredFile::getFileType()
{
    if (!m_pEditor)
        return "";
    FileType *pType = m_pEditor->getFileType();
    if (!pType)
        return "";
    return pType->getName()->getChars();
}


/** Match pair for specified position local screen.

    @param iLine        Line in the file
    @param iPos         Position in the line
    @return             Instance of CCheckPairInfo object
                        or null is there is no match
  */
CCheckPairInfo *CColoredFile::checkLocalPair(int iLine, int iPos)
{
    if (!m_pEditor)
        return 0;

    if (iLine < 0 || iLine >= m_pFile->getLinesCount())
        return 0;

    PairMatch *pInfo = m_pEditor->getPairMatch(iLine, iPos);

    if (!pInfo)
        return 0;

    m_pEditor->searchLocalPair(pInfo);

    if (pInfo->sline == -1 || pInfo->eline == -1)
    {
        m_pEditor->releasePairMatch(pInfo);
        return 0;
    }

    CCheckPairInfo *pOurInfo = new CCheckPairInfo(pInfo);

    m_pEditor->releasePairMatch(pInfo);
    return pOurInfo;

}

/** Match pair for specified position global in all file.

    @param iLine        Line in the file
    @param iPos         Position in the line
    @return             Instance of CCheckPairInfo object
                        or null is there is no match
  */
CCheckPairInfo *CColoredFile::checkGlobalPair(int iLine, int iPos)
{
    if (!m_pEditor)
        return 0;

    if (iLine < 0 || iLine >= m_pFile->getLinesCount())
        return 0;

    PairMatch *pInfo = m_pEditor->getPairMatch(iLine, iPos);

    if (!pInfo)
        return 0;

    m_pEditor->searchGlobalPair(pInfo);

    if (pInfo->sline == -1 || pInfo->eline == -1)
    {
        m_pEditor->releasePairMatch(pInfo);
        return 0;
    }

    CCheckPairInfo *pOurInfo = new CCheckPairInfo(pInfo);

    m_pEditor->releasePairMatch(pInfo);
    return pOurInfo;

}

/** Idle job. */
void CColoredFile::onIdle()
{
    if (m_pEditor != 0)
        m_pEditor->idleJob(1);
}


/** Constructor.

    @param pInfo        Colorer's region information.
  */
CColoringInfo::CColoringInfo(TPtrVector<string*> &aSpellingRegions, LineRegion *pInfo) :
m_aSpellingRegions(aSpellingRegions)
{
    m_pInfo = pInfo;
}

int CColoringInfo::setColoringInfo(LineRegion *pInfo)
{
    m_pInfo = pInfo;
    return 0;
}
/** Get start of region. */
int CColoringInfo::getStartPos() const
{
    return m_pInfo->start;
}

/** Get length of region.

    Can return -1 if region is not syntax region
    and colorer can't related it with real text.
  */
int CColoringInfo::getLength() const
{
    if (m_pInfo->end >= m_pInfo->start)
        return m_pInfo->end - m_pInfo->start;
    else
        return -1;
}

/** Is the block a part of syntax region?. */
bool CColoringInfo::isSyntaxRegion() const
{
    return m_pInfo->region != 0;
}
/** Return colored reggion or no*/
bool CColoringInfo::isColored() const
{
    return m_pInfo->rdef != 0 && m_pInfo->special == 0;
}
/** This regions speling
    */
bool CColoringInfo::isSpellingRegion() const
{
    for (unsigned int i = 0; i < m_aSpellingRegions.size(); i++)
    {
        string *szTemp1 = m_aSpellingRegions.get(i);
        const char *szTemp2 = getRegionName();
        //default region is text
        // m_pInfo->region->getName()->getChars() == NULL
        if (!strcmp(szTemp1->c_str(), getRegionName()))
            return true;
    }
    return false;
}

/** Get color of region*/
unsigned short CColoringInfo::getColor() const
{
    unsigned short uColor;
    // from color map
    uColor = (unsigned short)(m_pInfo->styled()->fore + (m_pInfo->styled()->back<<4));
    // if not defined used default editor color
    if (!m_pInfo->styled()->bfore)
        uColor = (uColor & 0xF0) + s_uForeGroundColor;
    if (!m_pInfo->styled()->bback)
        uColor = (uColor & 0xF) + (s_uBackGroundColor << 4);
    return uColor;
}

/** Get full name of syntax region.

    The method can return empty string
    if the block is not a syntax region.
  */
const char *CColoringInfo::getRegionName() const
{
    if (m_pInfo->region != 0)
        return m_pInfo->region->getName()->getChars();
    else
        return "";
}

/** Get base name of syntax region.

    The method can return empty string
    if the block is not a syntax region.
  */
const char *CColoringInfo::getBaseRegionName() const
{
    if (m_pInfo->region != 0)
    {
        const Region *pRegion = m_pInfo->region;
        while (true)
        {
            if (memicmp(pRegion->getName()->getChars(), "def:", 4) == 0)
                break;
            if (pRegion->getParent())
                pRegion = pRegion->getParent();
            else
                break;
        }
        return pRegion->getName()->getChars();

    }
    else
        return "";
}

/** Constructor.

    @param pInfo        Colorer's region information.
  */
CCheckPairInfo::CCheckPairInfo(PairMatch *pInfo)
{
    m_iStartLine = pInfo->sline;
    m_iEndLine = pInfo->eline;

    m_iStartPos = pInfo->start->start;
    m_iStartLength = pInfo->start->end - pInfo->start->start;

    m_iEndPos = pInfo->end->start;
    m_iEndLength = pInfo->end->end - pInfo->end->start;
}

/** Get line where is start of the pair*/
int CCheckPairInfo::getStartLine() const
{
    return m_iStartLine;
}

/** Get column where is start of the pair*/
int CCheckPairInfo::getStartPos() const
{
    return m_iStartPos;
}

/** Get length of pair start*/
int CCheckPairInfo::getStartLength() const
{
    return m_iStartLength;
}

/** Get line where is end of the pair*/
int CCheckPairInfo::getEndLine() const
{
    return m_iEndLine;
}

/** Get column where is end of the pair*/
int CCheckPairInfo::getEndPos() const
{
    return m_iEndPos;
}

/** Get length of pair end*/
int CCheckPairInfo::getEndLength() const
{
    return m_iEndLength;
}

/** Check is specified position at start of
    the pair.

    @param iLine        The line to be checked
    @param iPos         The columns the be checked
  */
bool CCheckPairInfo::isStartOfPair(int iLine, int iPos)
{
    if (iLine == m_iStartLine &&
        iPos >= m_iStartPos &&
        iPos < m_iStartPos + m_iStartLength)
        return true;
    else
        return false;
}

/** Check is specified position at end of
    the pair.

    @param iLine        The line to be checked
    @param iPos         The columns the be checked
  */
bool CCheckPairInfo::isEndOfPair(int iLine, int iPos)
{
    if (iLine == m_iEndLine &&
        iPos >= m_iEndPos &&
        iPos < m_iEndPos + m_iEndLength)
        return true;
    else
        return false;
}

/** Release object. */
void CCheckPairInfo::release()
{
    delete this;
}




