#include "stdhead.h"

#include "colorinfo.h"
#include "ConsoleIO.h"
#include "vectors/vectors.h"
#include "dialog.h"

#include "undo.h"
#include "XProfile.h"
#include "XDrawer.h"
#include "XEditor.h"
#include "XProfile.h"
#include "Menu.h"
#include "KeyMap.h"
#include "AutoSaver.h"
#include "XShell.h"
#include "Plugin.h"
#include "XBuilder.h"
#include "Speller.h"
#include "CSAPI.h"
#include "SyntaxProcessor.h"

CXBuilder::CXBuilder()
{
    // load ini file
    char szBuffer[256];
    GetModuleFileName(NULL, szBuffer, MAX_PATH);
    strcpy(szBuffer + strlen(szBuffer)-3,"ini");
    m_oProfile.setFileName(szBuffer);
}
/** Destructor */
CXBuilder::~CXBuilder()
{
}

/** Create auto saver
    */
CAutoSaver *CXBuilder::createAutoSaver(CXEditor *pEditor)
{
    CAutoSaver *pAutoSaver;
    string sSectionName;
    m_oProfile.setSectionName("common");
    int uTimeOut = m_oProfile.getAsInt("autosave", "0");
    pAutoSaver = NULL;
    if (uTimeOut > 0)
        pAutoSaver = new CAutoSaver(pEditor, uTimeOut * 1000);
    return pAutoSaver;
}

/** Load all color description.

    @param pColorer     Colorer
  */
void CXBuilder::loadColors(CSyntaxColorer *pColorer)
{
    string sSectionName;
    m_oProfile.setSectionName("common");

    sSectionName = m_oProfile.getAsString("color", "default");
    if (sSectionName != "")
        sSectionName += ".color";

    CColorInfo *pInfo = CColorInfo::getInstance();

    pInfo->setText(m_oProfile.getAsHexInt("colorText", "70"));
    pInfo->setBlockSelection(m_oProfile.getAsHexInt("colorTextBlock", "30"));
    pInfo->setSpeller(m_oProfile.getAsHexInt("colorSpell", "74"));
    pInfo->setPairHighLight(m_oProfile.getAsHexInt("colorPairHighlight", "72"));
    pInfo->setCurrentLine(m_oProfile.getAsHexInt("colorCurrentLine", "71"));
    pInfo->setCurrentBlock(m_oProfile.getAsHexInt("colorCurrentLineBlock", "31"));
    pInfo->setShellFolderSelect(m_oProfile.getAsHexInt("colorFolderSelect", "73"));
    pInfo->setColorInfoline(m_oProfile.getAsHexInt("colorInfo", "80"));
    pInfo->setColorQueryline(m_oProfile.getAsHexInt("colorQuery", "80"));
    pInfo->setMenuColor(m_oProfile.getAsHexInt("colorMenu", "80"));
    pInfo->setMenuSelect(m_oProfile.getAsHexInt("colorMenuSelect", "07"));
    pInfo->setDialog(m_oProfile.getAsHexInt("colorDialog", "80"));
    pInfo->setDialogActive(m_oProfile.getAsHexInt("colorDialogActive", "07"));
    pInfo->setDialogActiveSelect(m_oProfile.getAsHexInt("colorDialogActiveSelect", "70"));
    pInfo->setDialogInactiveSelect(m_oProfile.getAsHexInt("colorDialogInactiveSelect", "70"));

    unsigned short uColor;

    if (pColorer->getColorOfRegion("def:Text", &uColor))
        pInfo->setText(uColor);
    if (pColorer->getColorOfRegion("def:colorTextBlock", &uColor))
        pInfo->setBlockSelection(uColor);
    if (pColorer->getColorOfRegion("def:colorSpell", &uColor))
        pInfo->setSpeller(uColor);
    if (pColorer->getColorOfRegion("def:colorPairHighlight", &uColor))
        pInfo->setPairHighLight(uColor);
    if (pColorer->getColorOfRegion("def:colorCurrentLine", &uColor))
        pInfo->setCurrentLine(uColor);
    if (pColorer->getColorOfRegion("def:colorCurrentLineBlock", &uColor))
        pInfo->setCurrentBlock(uColor);
    if (pColorer->getColorOfRegion("def:colorFolderSelect", &uColor))
        pInfo->setShellFolderSelect(uColor);
    if (pColorer->getColorOfRegion("def:colorInfo", &uColor))
        pInfo->setColorInfoline(uColor);
    if (pColorer->getColorOfRegion("def:colorQuery", &uColor))
        pInfo->setColorQueryline(uColor);
    if (pColorer->getColorOfRegion("def:colorMenu", &uColor))
        pInfo->setMenuColor(uColor);
    if (pColorer->getColorOfRegion("def:colorMenuSelect", &uColor))
        pInfo->setMenuSelect(uColor);
    if (pColorer->getColorOfRegion("def:colorDialog", &uColor))
        pInfo->setDialog(uColor);
    if (pColorer->getColorOfRegion("def:colorDialogActive", &uColor))
        pInfo->setDialogActive(uColor);
    if (pColorer->getColorOfRegion("def:colorDialogSelect", &uColor))
        pInfo->setDialogActiveSelect(uColor);
    if (pColorer->getColorOfRegion("def:colorDialogInactiveSelect", &uColor))
        pInfo->setDialogInactiveSelect(uColor);
}

/** Initialization drawer options
        @param pDrawer - pointer to drawer object
                @param pInfoLine - Info line class
                @param pColorer - syntax colorer pointer, if not null
                                                  create drawer get colors for output from this class
*/
CXDrawer *CXBuilder::createDrawer(CConsoleOutput *pConsoleOutput,
                                  CInfoLine *pInfoLine)
{
    CXDrawer *pDrawer = new CXDrawer(pConsoleOutput, pInfoLine);
    return pDrawer;
}

CSyntaxColorer *CXBuilder::createColorer()
{
    m_oProfile.setSectionName("common");

    int iUseCololer = m_oProfile.getAsInt("colorer", "0");

    if (iUseCololer  == 0)
        return 0;

    string szCatalogName;
    string szHrdClass;
    string szHrdName;

    szCatalogName = m_oProfile.getAsString("HRCPATH", "colorer\\catalog.xml");
    szHrdClass = m_oProfile.getAsString("HrdClass", "console");
    szHrdName  = m_oProfile.getAsString("HrdName", "gray");

    int iBackParse = m_oProfile.getAsInt("backParse", "2000");
        int iMaxLen = m_oProfile.getAsInt("maxLenLine", "5000");

    char szBuffer[MAX_PATH];
    _fullpath(szBuffer, szCatalogName.c_str(), MAX_PATH);

    CSyntaxColorer *pColorer = new CSyntaxColorer(*this);
    if (!pColorer->init(szBuffer, szHrdClass.c_str(), szHrdName.c_str(), iBackParse, iMaxLen))
    {
            CConsoleDialog::ColorInfo cInfo;
            cInfo.clrBg = 0x80;
            cInfo.clrInactive = 0x80;
            cInfo.clrActive = 0x70;
            cInfo.clrTextInactive = 0x80;
            cInfo.clrTextActive = 0x80;
            cInfo.clrSelInactive = 0x80;
            cInfo.clrSelActive = 0x70;

            CStandardBox::message("XCE", "Colorer initialization failed", CStandardBox::eOK, &cInfo);
            delete pColorer;
            return NULL;
    }
        return pColorer;
}
/** Initialization info line object
        @param pConsole         output console object for menu
  */
CInfoLine *CXBuilder::createInfoLine(CConsoleOutput *pConsole)
{
    return new CInfoLine(pConsole,
                         0, 0,
                         pConsole->getScreenColumns(),
                         CColorInfo::getInstance()->getColorInfoline());
}


ISpeller *CXBuilder::createSpeller()
{
    m_oProfile.setSectionName("common");
    string sSpellerName = m_oProfile.getAsString("speller_name", "");
    string sExcludeWord = m_oProfile.getAsString("exclude_words", "xce.spe");

    if (sSpellerName == "dictionary")
    {
        CSpellerEngine *pEngine;

        pEngine = new CSpellerEngine(sExcludeWord.c_str());
                //*** ng: correct slashes: CSpeller doesn't understand direct slash
        if (pEngine->init(".\\dictionary\\"))
            return static_cast<ISpeller*>(pEngine);
        else
            return NULL;
    }

    if (sSpellerName == "csapi")
    {
        CCSAPISpellerEngine *pEngine;

        pEngine = new CCSAPISpellerEngine (sExcludeWord.c_str());
        if(pEngine->init())
            return static_cast<ISpeller*>(pEngine);
        else
            return NULL;
    }

    return NULL;

}

/** Initialization menu
    @param aMenus           list of menu
    @param pConsole         output console object for menu
*/
CMenu *CXBuilder::createMenu(const CXKeyMap &KeyMap)
{
    int nMenuWidth = 26;
    string sSectionName;
    CMenu *pMenu;

    pMenu = new CMenu(1, 0);

    string sMenuSectionName;
    m_oProfile.setSectionName("common");
    sMenuSectionName = m_oProfile.getAsString("menu", "");

    if (sMenuSectionName != "")
        sMenuSectionName += ".menu";
    else
        sMenuSectionName = "menu";

    m_oProfile.setSectionName(sMenuSectionName);
    vector<pair<string, pair<string, string> > > vSection;
    m_oProfile.getSection(vSection);

    if (vSection.size() == 0)
        // default key map load
        initDefaultMenu(pMenu, nMenuWidth, KeyMap);
    else
        // decode returned string to key map
        parseMenuBuffer(vSection, pMenu, nMenuWidth, KeyMap);
    return pMenu;
}


/** Initialization menu
    @oEditor - reference to object what initializations
*/
void CXBuilder::createEditor(/*vector& aFilesToOpen,*/ CXEditor& oEditor)
{
}

/** Init key map
    @param                Key Map Object
*/
void CXBuilder::initKeyMap(CXKeyMap &KeyMap)
{
    string sKeyMapSectionName;
    char szKeyMapBuffer[32767]; // max size for winX OS
    m_oProfile.setSectionName("common");
    sKeyMapSectionName = m_oProfile.getAsString("keymap", "");

    if (sKeyMapSectionName != "")
        sKeyMapSectionName += ".keymap";
    else
        sKeyMapSectionName = "keymap";

    m_oProfile.setSectionName(sKeyMapSectionName);
    m_oProfile.getSection(szKeyMapBuffer);

    if (strlen(szKeyMapBuffer) == 0)
        // default key map load
        initDefaultKeyMap(KeyMap);
    else
        // decode returned string to key map
        parseKeyBuffer(szKeyMapBuffer, KeyMap);

}

/** Parse buffer with key map data, getting from ini file.
    @param szBuffer        buffer for parsing
    @param KeyMap           Key Map Object
*/

void CXBuilder::parseKeyBuffer(const char *szKeyMapBuffer, CXKeyMap &KeyMap)
{
    // must be init it's strcu
    Event oEvent = {0};
    char szBuffer[256];

    int i = 0;
    while (*(szKeyMapBuffer + i))
    {
        oEvent.bAlt = false;
        oEvent.bCtrl = false;
        oEvent.bShift = false;

        if (*(szKeyMapBuffer + i) == ';')
        {
            // comment skip line
            i += strlen(szKeyMapBuffer + i) + 1;
            continue;
        }

       // get string to =
        char *q = const_cast<char *>(strchr(szKeyMapBuffer + i , '='));

        if (q)
            *q = 0;


        if (CXKeyMap::decodeKey(szKeyMapBuffer + i, &oEvent))
        {

            i += strlen(szKeyMapBuffer + i) + 1;
            q = const_cast<char *>(strchr(szKeyMapBuffer + i, ','));
            if (q)
                *q = 0;
            strcpy(szBuffer, szKeyMapBuffer + i);
            if (q)
                *q = ' ';

            if (CXKeyMap::decodeCommand(szBuffer, &oEvent))
            {
                if (q)
                    strcpy(oEvent.idCommand.szCommandInfo, q + 1);
                else
                    oEvent.idCommand.szCommandInfo[0] = 0;
                KeyMap.addEvent(oEvent);
            }
            else
            {
                char szMsg[MAX_PATH];
                sprintf (szMsg, "%s unknown command\n", szBuffer);
                CConsoleDialog::ColorInfo cInfo;
                cInfo.clrBg = 0x80;
                cInfo.clrInactive = 0x80;
                cInfo.clrActive = 0x70;
                cInfo.clrTextInactive = 0x80;
                cInfo.clrTextActive = 0x80;
                cInfo.clrSelInactive = 0x80;
                cInfo.clrSelActive = 0x70;

                CStandardBox::message("XCE", szMsg, CStandardBox::eOK, &cInfo);
            }

            if (q)
                i = q - szKeyMapBuffer + strlen(q) + 1;
            else
                i += strlen(szKeyMapBuffer + i) + 1;
        }
        else
        {
            char szMsg[MAX_PATH];
            sprintf (szMsg, "%s undefined key\n", szKeyMapBuffer + i);
            CConsoleDialog::ColorInfo cInfo;
            cInfo.clrBg = 0x80;
            cInfo.clrInactive = 0x80;
            cInfo.clrActive = 0x70;
            cInfo.clrTextInactive = 0x80;
            cInfo.clrTextActive = 0x80;
            cInfo.clrSelInactive = 0x80;
            cInfo.clrSelActive = 0x70;
            CStandardBox::message("XCE", szMsg, CStandardBox::eOK, &cInfo);

            i += strlen(szKeyMapBuffer + i) + 1;
            q = const_cast<char *>(strchr(szKeyMapBuffer + i, ','));
            if (q)
                *q = 0;
            strcpy(szBuffer, szKeyMapBuffer + i);
            if (q)
                *q = ' ';

            if (q)
                i = q - szKeyMapBuffer + strlen(q) + 1;
            else
                i += strlen(szKeyMapBuffer + i) + 1;
        }
    }

}

/** Default key map
    @param                Key Map Object
*/
void CXBuilder::initDefaultKeyMap(CXKeyMap &KeyMap)
{
    Event evDefault;

    // Cursor command
    evDefault.bAlt = false;
    evDefault.bCtrl = false;
    evDefault.bShift = false;
    evDefault.uScan = scLeft;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdLeft;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scRight;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdRight;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scUp;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdUp;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scDown;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdDown;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scPgUp;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdPageUp;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scPgDown;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdPageDown;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scHome;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdLineStart;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scEnd;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdLineEnd;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scPgUp;
    evDefault.bCtrl  = true;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdFileStart;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scPgDown;
    evDefault.bCtrl  = true;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdFileEnd;
    KeyMap.addEvent(evDefault);

    evDefault.bCtrl  = false;

    evDefault.uScan = scBackspace;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBackspace;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scDel;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdDelete;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scEnter;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdEnter;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scInsert;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdChangeOverwriteMode;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scTab;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdTab;
    KeyMap.addEvent(evDefault);



    evDefault.uScan = CXKeyMap::getScanByDesc("Y");
    evDefault.bCtrl  = true;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdDeleteLine;
    KeyMap.addEvent(evDefault);


    evDefault.uScan = scLeft;
    evDefault.bCtrl  = true;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdWordLeft;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scRight;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdWordRight;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scDel;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdDeleteWord;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scBackspace;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBackspaceWord;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scTab;
    evDefault.bCtrl  = false;
    evDefault.bShift = true;
    evDefault.idCommand.eType = Command::Move;
    evDefault.idCommand.iIdentity = cmdTabBack;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF9;
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockUnmark;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF7;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockMarkLine;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF7;
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockMarkColumn;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF7;
    evDefault.bCtrl  = false;
    evDefault.bShift = true;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockMarkStream;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scInsert;
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockCopyClipboard;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = CXKeyMap::getScanByDesc("X");
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockCutClipboard;
    KeyMap.addEvent(evDefault);


    evDefault.uScan = scF9;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockCopy;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF10;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockMove;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scDel;
    evDefault.bCtrl  = false;
    evDefault.bShift = true;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockDelete;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scInsert;
    evDefault.bCtrl  = false;
    evDefault.bShift = true;;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdBlockPaste;
    KeyMap.addEvent(evDefault);


    evDefault.uScan = scF3;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = true;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdGoLine;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = CXKeyMap::getScanByDesc("M");
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdSetMarker;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = CXKeyMap::getScanByDesc("G");
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdGoMarker;
    KeyMap.addEvent(evDefault);


    // Search hot key
    evDefault.uScan = scF6;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdSearch;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF6;
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdSearchNext;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF6;
    evDefault.bCtrl  = false;
    evDefault.bShift = true;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdReplace;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = CXKeyMap::getScanByDesc("P");;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = true;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdChangeCodePage;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = CXKeyMap::getScanByDesc("S");;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = true;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdChangeSpellMode;



    evDefault.uScan = scEsc;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::EditorWide;
    evDefault.idCommand.iIdentity = cmdActivateMenu;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF4;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = true;

    evDefault.idCommand.eType = Command::EditorWide;
    evDefault.idCommand.iIdentity = cmdExit;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF2;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdSave;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF2;
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::EditorWide;
    evDefault.idCommand.iIdentity = cmdOpenFile;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF2;
    evDefault.bCtrl  = false;
    evDefault.bShift = true;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::EditorWide;
    evDefault.idCommand.iIdentity = cmdOpenFolder;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = CXKeyMap::getScanByDesc("S");
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::EditorWide;
    evDefault.idCommand.iIdentity = cmdSaveBlock;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = scF4;
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = false;

    evDefault.idCommand.eType = Command::EditorWide;
    evDefault.idCommand.iIdentity = cmdClose;
    KeyMap.addEvent(evDefault);

    evDefault.uScan = CXKeyMap::getScanByDesc("X");
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = true;



    evDefault.idCommand.eType = Command::EditorWide;
    evDefault.idCommand.iIdentity = cmdNextWindow;
    KeyMap.addEvent(evDefault);


    evDefault.uScan = scF4;
    evDefault.bCtrl  = true;
    evDefault.bShift = false;
    evDefault.bAlt   = false;
    evDefault.idCommand.eType = Command::Shell;
    evDefault.idCommand.iIdentity = cmdGotoWindow;

    KeyMap.addEvent(evDefault);

    evDefault.uScan = CXKeyMap::getScanByDesc("S");
    evDefault.bCtrl  = false;
    evDefault.bShift = true;
    evDefault.bAlt   = true;
    evDefault.idCommand.eType = Command::Shell;
    evDefault.idCommand.iIdentity = cmdSpellSuggest;

    evDefault.uScan = CXKeyMap::getScanByDesc("W");
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = true;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdInterwindowCopy;

    evDefault.uScan = CXKeyMap::getScanByDesc("M");
    evDefault.bCtrl  = false;
    evDefault.bShift = false;
    evDefault.bAlt   = true;
    evDefault.idCommand.eType = Command::WindowWide;
    evDefault.idCommand.iIdentity = cmdInterwindowMove;

    KeyMap.addEvent(evDefault);


}
/** Build menu from ini file section
    @param vSection         vector of parsing section
    @param pMenu            shell menu
    @param pConsole         output console object for menu
    @param uWidth,          width of menu
    @param uColor           menu color
    @param uColorSelect     select item color
    @param KeyMap           current KeyMap for this application
*/
void CXBuilder::parseMenuBuffer(vector<pair<string, pair<string, string> > > &vSection,
                                CMenu *pMenu,
                                unsigned short uWidth,
                                const CXKeyMap &KeyMap)
{

    CSubMenu *pSubMenu = 0;
    menuitem miItem;

    for (int i = 0; i < vSection.size(); i++)
    {
        if (!strcmpi(vSection[i].second.first.c_str(), "Section"))
        {
            if (pSubMenu)
                pMenu->addSubMenu(pSubMenu);

            pSubMenu = new CSubMenu(uWidth, vSection[i].first.c_str());
        } else
        {
            createMenuItem(vSection[i].first.c_str(), vSection[i].second.first.c_str(),
                           vSection[i].second.second.c_str(), uWidth, miItem, KeyMap);
            pSubMenu->addItem(miItem);
        }
    }
    if (pSubMenu)
        pMenu->addSubMenu(pSubMenu);
}



/** Default menu
    @param aMenus                 list of menu
    @param uWidth                 width of menu
    @param uColor                 color of menu
    @param uColorSelect           color of selected item
*/
void CXBuilder::initDefaultMenu(CMenu *pMenu,
                                unsigned short uWidth,
                                const CXKeyMap &KeyMap)

{
    unsigned short iIndex;
    menuitem miItem;
    CSubMenu *pSubMenu;

    // file menu
    pSubMenu = new CSubMenu(uWidth,
                            "File");
    for (iIndex =0; mFile[iIndex].szPromt[0] != 0; iIndex++)
    {
        createMenuItem(mFile[iIndex].szPromt,
                       mFile[iIndex].szCommandDescription,
                       mFile[iIndex].szCommandParams,
                       uWidth,
                       miItem,
                       KeyMap);
        pSubMenu->addItem(miItem);
    }
    pMenu->addSubMenu(pSubMenu);

    // edit menu
    pSubMenu = new CSubMenu(uWidth,
                            "Edit");
    for (iIndex =0; mEdit[iIndex].szPromt[0] != 0; iIndex++)
    {
        createMenuItem(mEdit[iIndex].szPromt,
                       mEdit[iIndex].szCommandDescription,
                       mEdit[iIndex].szCommandParams,
                       uWidth,
                       miItem,
                       KeyMap);
        pSubMenu->addItem(miItem);
    }
    pMenu->addSubMenu(pSubMenu);

    // text menu
    pSubMenu = new CSubMenu(uWidth,
                            "Text");
    for (iIndex =0; mText[iIndex].szPromt[0] != 0; iIndex++)
    {
        createMenuItem(mText[iIndex].szPromt,
                       mText[iIndex].szCommandDescription,
                       mText[iIndex].szCommandParams,
                       uWidth,
                       miItem,
                       KeyMap);
        pSubMenu->addItem(miItem);
    }
    pMenu->addSubMenu(pSubMenu);
}

void CXBuilder::createMenuItem(const char *szPrompt,
                               const char *szCommandDesc,
                               const char *szCommandParams,
                               unsigned short uWidth,
                               menuitem &mItem,
                               const CXKeyMap &KeyMap)
{

    Event cmdEvent = {0};
    char szCommandUpperDesc[32];
    // upper case
    strcpy(szCommandUpperDesc, szCommandDesc);
    _strupr(szCommandUpperDesc);

    const char *szKeyDecription;
    char szHotKey[6] = "";
    unsigned short uLen, i;
    uLen = 0;

    // create prompt
    if(!strcmp(szCommandUpperDesc, "NOTHING") )
    {
        strcpy(mItem.szPrompt, "");
        mItem.cmd.eType = Command::None;
        return;
    }
    strncpy(mItem.szPrompt, szPrompt,  uWidth);

    // decode comand
    if (!CXKeyMap::decodeCommand(szCommandUpperDesc, &cmdEvent))
    {
         char szMsg[MAX_PATH];
         sprintf (szMsg, "%s unknown command\n", szCommandUpperDesc);
         CConsoleDialog::ColorInfo cInfo;
         cInfo.clrBg = 0x80;
         cInfo.clrInactive = 0x80;
         cInfo.clrActive = 0x70;
         cInfo.clrTextInactive = 0x80;
         cInfo.clrTextActive = 0x80;
         cInfo.clrSelInactive = 0x80;
         cInfo.clrSelActive = 0x70;

         CStandardBox::message("XCE", szMsg, CStandardBox::eOK, &cInfo);
    }

    if (cmdEvent.idCommand.eType != Command::None)
    {
        strcpy(cmdEvent.idCommand.szCommandInfo, szCommandParams);
        // get Hot key
        KeyMap.getKeyByCommand(&cmdEvent);
        mItem.cmd = cmdEvent.idCommand;
        // add hot key
        if (cmdEvent.bCtrl)
            strcat(szHotKey, "C");

        if (cmdEvent.bAlt)
            strcat(szHotKey, "A");

        if (cmdEvent.bShift)
            strcat(szHotKey, "S");

        if (strlen(szHotKey))
            strcat(szHotKey, "-");

        szKeyDecription = CXKeyMap::getDescByScan(cmdEvent.uScan);
        if (szKeyDecription != NULL)
            strcat(szHotKey, szKeyDecription);
        if (strlen(szHotKey) != NULL)
        {
            // fill space to end string
            uLen = strlen(mItem.szPrompt);
            for ( i = uLen; i < uWidth; i++)
                mItem.szPrompt[i] =  ' ';

            // copy hot key description
            memcpy(&mItem.szPrompt[uWidth - strlen(szHotKey) -1],
                   szHotKey,
                   strlen(szHotKey) + 1);
        }
    }

}

/** Get search replace data from persist storage*/
void CXBuilder::getSearchData(char *szSearch, char *szReplace, int &iMode)
{
    string szBuffer;
    m_oProfile.setSectionName("common");
    szBuffer = m_oProfile.getAsString("search","");
    strcpy(szSearch, szBuffer.c_str());
    szBuffer = m_oProfile.getAsString("replace","");
    strcpy(szReplace, szBuffer.c_str());

    iMode = m_oProfile.getAsInt("search_type","0");
}

/** Set search replace data from persist storage*/
void CXBuilder::setSearchData(const char *szSearch, const char *szReplace, int iMode)
{
    m_oProfile.setSectionName("common");
    m_oProfile.setAsString("search", (szSearch != NULL) ? szSearch : "");
    m_oProfile.setAsString("replace", (szReplace != NULL) ? szReplace : "");
    string szSearchData;
    char sTmp[10];
    itoa(iMode, sTmp, 10);
    szSearchData = sTmp;
    m_oProfile.setAsString("search_type", szSearchData);
}


/** Get help string.
    @param szFileExt*/

const char *CXBuilder::getHelp(const char *szFileExt)
{
    return NULL;
}


/** Get spelling region name.
    @param aRegions        array of regions name need for spelling
*/
bool CXBuilder::getSpellingRegions(const char *szFileName, TPtrVector<string*> &aRegions)
{
    int i, nLen;
    nLen = strlen(szFileName);

    for (i = nLen - 1; i >= 0 && szFileName[i] != '.' && szFileName[i] != '\\' && szFileName[i] != '/'; i--);
    if (szFileName[i] == '.')
    {
        i = i + 1;

        m_oProfile.setSectionName(szFileName + i);
        string upload = m_oProfile.getAsString("upload", "noupload");
        if (strcmp(upload.c_str(), "noupload"))
        {
            m_oProfile.setSectionName(upload.c_str());
        }


        aRegions.clear();
        string sRegion = m_oProfile.getAsString("RegionsForSpelling", "No Spelling");
        if (strcmp(sRegion.c_str(), "No Spelling"))
        {
                CStringSlicer Slicer("/\\w+:\\w+/");
                int iStart = 0, iEnd =0;
                const char *szWord;

                while ( (szWord = Slicer.sliceWord(sRegion.c_str(), iStart, iEnd)) != NULL)
                {
                        string* szRegion = new string(szWord, iEnd - iStart);
                        //szRegion;
                        //strncpy(szTemp, szWord, iEnd - iStart+1);
                        //szTemp[iEnd - iStart] = 0;

                        if (!strcmp("def:default", szRegion->c_str()))
                                aRegions.add(new string(""));
                        else
                                aRegions.add(szRegion);

                        iStart = iEnd;
                }
                return true;
        }
    }
    return false;
}

/** Get integer option */
int CXBuilder::getOption(const char *szSection, const char *szKey, int iDefVal)
{
    m_oProfile.setSectionName(szSection);
    const char *szDef = "???";
    std::string s = m_oProfile.getAsString(szKey, szDef);
    if (!strcmp(szDef, s.c_str()))
        return iDefVal;
    else
        return atoi(s.c_str());
}




/** Create plugin manager.
*/
CPluginManager *CXBuilder::createPluginManager(CXEditor *pEditor)
{
    string sPath;
    m_oProfile.setSectionName("common");
    sPath = m_oProfile.getAsString("plugin_path", ".//plugin");
    CPluginManager *pPluginManager;
    try
    {
        pPluginManager = new CPluginManager(pEditor, sPath.c_str());
    }
    catch (...)
    {
        return NULL;
    }

    return pPluginManager;
}

