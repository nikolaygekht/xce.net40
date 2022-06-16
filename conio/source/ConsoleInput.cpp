#pragma unmanaged
#include "stdhead.h"

unsigned int htoi (const char *ptr)
{
    unsigned int value = 0;
    char ch = *ptr;

    while (ch == ' ' || ch == '\t')
        ch = *(++ptr);

    for (;;) {

        if (ch >= '0' && ch <= '9')
            value = (value << 4) + (ch - '0');
        else if (ch >= 'A' && ch <= 'F')
            value = (value << 4) + (ch - 'A' + 10);
        else if (ch >= 'a' && ch <= 'f')
            value = (value << 4) + (ch - 'a' + 10);
        else
            return value;
        ch = *(++ptr);
    }
}


#pragma managed
#using <mscorlib.dll>
using namespace System;
using namespace System::Runtime::InteropServices;

#include "ConsoleInput.h"

namespace gehtsoft
{
namespace xce
{
namespace conio
{

ScanCode::ScanCode()
{
}

ConsoleInput::ConsoleInput()
{
    SetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), ENABLE_MOUSE_INPUT);
    mLeftButtonPressed = false;
    mRightButtonPressed = false;
    mMouseRow = -1;
    mMouseColumn = -1;

    HINSTANCE hInstance = ::LoadLibrary(L"kernel32.dll");
    GetConsoleKeyboardLayoutNameA = (PGetConsoleKeyboardLayoutNameA)::GetProcAddress(hInstance, "GetConsoleKeyboardLayoutNameA");

}

int ConsoleInput::CurrentLayout::get()
{
    char b[256];
    if (GetConsoleKeyboardLayoutNameA != 0)
    {
        GetConsoleKeyboardLayoutNameA(b);
        int code = htoi(b);
        return code;
    }
    else
        return -1;
}

ConsoleInput::~ConsoleInput()
{
    this->!ConsoleInput();
}

ConsoleInput::!ConsoleInput()
{
}


bool ConsoleInput::read(ConsoleInputListener ^listener, int timeout)
{
    unsigned long ulEvents;
    INPUT_RECORD ri;

    ulEvents = 0;
    if (WaitForSingleObject(GetStdHandle(STD_INPUT_HANDLE), timeout) != WAIT_OBJECT_0)
        return false;

    PeekConsoleInput(GetStdHandle(STD_INPUT_HANDLE), &ri, 1, &ulEvents);
    switch(ri.EventType)
    {
    case    KEY_EVENT:          //keyboard event
            {
                bool shift = (ri.Event.KeyEvent.dwControlKeyState & SHIFT_PRESSED) != 0;
                bool alt = (ri.Event.KeyEvent.dwControlKeyState & LEFT_ALT_PRESSED) != 0 || (ri.Event.KeyEvent.dwControlKeyState & RIGHT_ALT_PRESSED) != 0;
                bool ctrl = (ri.Event.KeyEvent.dwControlKeyState & LEFT_CTRL_PRESSED) != 0 || (ri.Event.KeyEvent.dwControlKeyState & RIGHT_CTRL_PRESSED) != 0;
                int scanCode = 0;
                wchar_t chr = 0;
                bool press = false;

                if (ri.Event.KeyEvent.bKeyDown && ri.Event.KeyEvent.wRepeatCount == 1)
                {
                    ReadConsoleInputW(GetStdHandle(STD_INPUT_HANDLE), &ri, 1, &ulEvents);
                    scanCode = ri.Event.KeyEvent.wVirtualKeyCode;
                    chr = ri.Event.KeyEvent.uChar.UnicodeChar;
                    press = true;
                }
                else if (!ri.Event.KeyEvent.bKeyDown)
                {
                    ReadConsoleInputW(GetStdHandle(STD_INPUT_HANDLE), &ri, 1, &ulEvents);
                    scanCode = ri.Event.KeyEvent.wVirtualKeyCode;
                    chr = ri.Event.KeyEvent.uChar.UnicodeChar;
                    press = false;
                }
                else
                {
                    GetNumberOfConsoleInputEvents(GetStdHandle(STD_INPUT_HANDLE), &ulEvents);
                    if (ulEvents)
                        ReadConsoleInput(GetStdHandle(STD_INPUT_HANDLE), &ri, 1, &ulEvents);
                }

                if (scanCode != 0)
                {
                    if (press)
                        listener->onKeyPressed(scanCode, chr, shift, ctrl, alt);
                    else
                        listener->onKeyReleased(scanCode, chr, shift, ctrl, alt);
                    return true;
                }
                else
                    return false;
            }
    case    MOUSE_EVENT:    //mouse input event
            {
                GetNumberOfConsoleInputEvents(GetStdHandle(STD_INPUT_HANDLE), &ulEvents);
                if (ulEvents)
                    ReadConsoleInputW(GetStdHandle(STD_INPUT_HANDLE), &ri, 1, &ulEvents);

                bool shift = (ri.Event.MouseEvent.dwControlKeyState & SHIFT_PRESSED) != 0;
                bool alt = (ri.Event.MouseEvent.dwControlKeyState & LEFT_ALT_PRESSED) != 0 || (ri.Event.MouseEvent.dwControlKeyState & RIGHT_ALT_PRESSED) != 0;
                bool ctrl = (ri.Event.MouseEvent.dwControlKeyState & LEFT_CTRL_PRESSED) != 0 || (ri.Event.MouseEvent.dwControlKeyState & RIGHT_CTRL_PRESSED) != 0;

                switch (ri.Event.MouseEvent.dwEventFlags)
                {
                case    0:
                case    DOUBLE_CLICK:
                case    MOUSE_MOVED:
                        {
                            int row, column;
                            bool left, right, rc;
                            column = ri.Event.MouseEvent.dwMousePosition.X;
                            row = ri.Event.MouseEvent.dwMousePosition.Y;
                            left = (ri.Event.MouseEvent.dwButtonState & FROM_LEFT_1ST_BUTTON_PRESSED) != 0;
                            right = (ri.Event.MouseEvent.dwButtonState & RIGHTMOST_BUTTON_PRESSED) != 0;
                            rc = false;
                            bool oldLeftButtonPressed = mLeftButtonPressed;
                            bool oldRightButtonPressed = mRightButtonPressed;
                            mLeftButtonPressed = left;
                            mRightButtonPressed = right;

                            if (left != oldLeftButtonPressed)
                            {
                                if (left)
                                    listener->onMouseLButtonDown(row, column, shift, ctrl, alt);
                                else
                                    listener->onMouseLButtonUp(row, column, shift, ctrl, alt);
                                rc = true;
                            }

                            if (right != oldRightButtonPressed)
                            {
                                if (right)
                                    listener->onMouseRButtonDown(row, column, shift, ctrl, alt);
                                else
                                    listener->onMouseRButtonUp(row, column, shift, ctrl, alt);
                                rc = true;
                            }

                            if (!rc && (row != mMouseRow || column != mMouseColumn))
                            {
                                listener->onMouseMove(row, column, shift, ctrl, alt, left, right);
                                rc = true;
                            }

                            mMouseRow = row;
                            mMouseColumn = column;
                            return rc;
                        }
                case    MOUSE_WHEELED:
                        {
                            int row, column;
                            column = ri.Event.MouseEvent.dwMousePosition.X;
                            row = ri.Event.MouseEvent.dwMousePosition.Y;

                            if (ri.Event.MouseEvent.dwButtonState & 0xFF000000)
                                listener->onMouseWheelDown(row, column, shift, ctrl, alt);
                            else
                                listener->onMouseWheelUp(row, column, shift, ctrl, alt);
                            return true;
                        }
                default:
                        return false;
                }
            }
    case    FOCUS_EVENT:    //out console window got focus
            {
                GetNumberOfConsoleInputEvents(GetStdHandle(STD_INPUT_HANDLE), &ulEvents);
                if (ulEvents)
                    ReadConsoleInputW(GetStdHandle(STD_INPUT_HANDLE), &ri, 1, &ulEvents);
                bool ctrl = (::GetKeyState(VK_CONTROL) & 0x8000) != 0;
                bool alt = (::GetKeyState(VK_MENU) & 0x8000) != 0;
                bool shift = (::GetKeyState(VK_SHIFT) & 0x8000) != 0;
                listener->onGetFocus(shift, ctrl, alt);
                return true;
            }
    case    WINDOW_BUFFER_SIZE_EVENT:
            {
                GetNumberOfConsoleInputEvents(GetStdHandle(STD_INPUT_HANDLE), &ulEvents);
                if (ulEvents)
                    ReadConsoleInputW(GetStdHandle(STD_INPUT_HANDLE), &ri, 1, &ulEvents);
                listener->onScreenBufferChanged(ri.Event.WindowBufferSizeEvent.dwSize.Y, ri.Event.WindowBufferSizeEvent.dwSize.X);
                return true;
            }
    default:
            {
                GetNumberOfConsoleInputEvents(GetStdHandle(STD_INPUT_HANDLE), &ulEvents);
                if (ulEvents)
                    ReadConsoleInputW(GetStdHandle(STD_INPUT_HANDLE), &ri, 1, &ulEvents);
                return false;
            }
    }
}

int ConsoleInput::keyNameToCode(String ^name)
{
    int rc = -1;

    if (name == L"LBUTTON") rc = ScanCode::LBUTTON;
    else if (name == L"RBUTTON") rc = ScanCode::RBUTTON;
    else if (name == L"CANCEL") rc = ScanCode::CANCEL;
    else if (name == L"MBUTTON") rc = ScanCode::MBUTTON;
    else if (name == L"XBUTTON1") rc = ScanCode::XBUTTON1;
    else if (name == L"XBUTTON2") rc = ScanCode::XBUTTON2;
    else if (name == L"BACK") rc = ScanCode::BACK;
    else if (name == L"TAB") rc = ScanCode::TAB;
    else if (name == L"CLEAR") rc = ScanCode::CLEAR;
    else if (name == L"RETURN") rc = ScanCode::RETURN;
    else if (name == L"SHIFT") rc = ScanCode::SHIFT;
    else if (name == L"CONTROL") rc = ScanCode::CONTROL;
    else if (name == L"MENU") rc = ScanCode::MENU;
    else if (name == L"PAUSE") rc = ScanCode::PAUSE;
    else if (name == L"CAPITAL") rc = ScanCode::CAPITAL;
    else if (name == L"KANA") rc = ScanCode::KANA;
    else if (name == L"JUNJA") rc = ScanCode::JUNJA;
    else if (name == L"FINAL") rc = ScanCode::FINAL;
    else if (name == L"HANJA") rc = ScanCode::HANJA;
    else if (name == L"ESCAPE") rc = ScanCode::ESCAPE;
    else if (name == L"CONVERT") rc = ScanCode::CONVERT;
    else if (name == L"NONCONVERT") rc = ScanCode::NONCONVERT;
    else if (name == L"ACCEPT") rc = ScanCode::ACCEPT;
    else if (name == L"MODECHANGE") rc = ScanCode::MODECHANGE;
    else if (name == L"SPACE") rc = ScanCode::SPACE;
    else if (name == L"PRIOR") rc = ScanCode::PRIOR;
    else if (name == L"NEXT") rc = ScanCode::NEXT;
    else if (name == L"END") rc = ScanCode::END;
    else if (name == L"HOME") rc = ScanCode::HOME;
    else if (name == L"LEFT") rc = ScanCode::LEFT;
    else if (name == L"UP") rc = ScanCode::UP;
    else if (name == L"RIGHT") rc = ScanCode::RIGHT;
    else if (name == L"DOWN") rc = ScanCode::DOWN;
    else if (name == L"SELECT") rc = ScanCode::SELECT;
    else if (name == L"PRINT") rc = ScanCode::PRINT;
    else if (name == L"EXECUTE") rc = ScanCode::EXECUTE;
    else if (name == L"SNAPSHOT") rc = ScanCode::SNAPSHOT;
    else if (name == L"INSERT") rc = ScanCode::INSERT;
    else if (name == L"DEL") rc = ScanCode::DEL;
    else if (name == L"HELP") rc = ScanCode::HELP;
    else if (name == L"LWIN") rc = ScanCode::LWIN;
    else if (name == L"RWIN") rc = ScanCode::RWIN;
    else if (name == L"APPS") rc = ScanCode::APPS;
    else if (name == L"SLEEP") rc = ScanCode::SLEEP;
    else if (name == L"NUMPAD0") rc = ScanCode::NUMPAD0;
    else if (name == L"NUMPAD1") rc = ScanCode::NUMPAD1;
    else if (name == L"NUMPAD2") rc = ScanCode::NUMPAD2;
    else if (name == L"NUMPAD3") rc = ScanCode::NUMPAD3;
    else if (name == L"NUMPAD4") rc = ScanCode::NUMPAD4;
    else if (name == L"NUMPAD5") rc = ScanCode::NUMPAD5;
    else if (name == L"NUMPAD6") rc = ScanCode::NUMPAD6;
    else if (name == L"NUMPAD7") rc = ScanCode::NUMPAD7;
    else if (name == L"NUMPAD8") rc = ScanCode::NUMPAD8;
    else if (name == L"NUMPAD9") rc = ScanCode::NUMPAD9;
    else if (name == L"GRAY*") rc = ScanCode::MULTIPLY;
    else if (name == L"GRAY+") rc = ScanCode::ADD;
    else if (name == L"SEPARATOR") rc = ScanCode::SEPARATOR;
    else if (name == L"GRAY-") rc = ScanCode::SUBTRACT;
    else if (name == L"GRAY.") rc = ScanCode::DECIMAL;
    else if (name == L"GRAY/") rc = ScanCode::DIVIDE;
    else if (name == L"F1") rc = ScanCode::F1;
    else if (name == L"F2") rc = ScanCode::F2;
    else if (name == L"F3") rc = ScanCode::F3;
    else if (name == L"F4") rc = ScanCode::F4;
    else if (name == L"F5") rc = ScanCode::F5;
    else if (name == L"F6") rc = ScanCode::F6;
    else if (name == L"F7") rc = ScanCode::F7;
    else if (name == L"F8") rc = ScanCode::F8;
    else if (name == L"F9") rc = ScanCode::F9;
    else if (name == L"F10") rc = ScanCode::F10;
    else if (name == L"F11") rc = ScanCode::F11;
    else if (name == L"F12") rc = ScanCode::F12;
    else if (name == L"F13") rc = ScanCode::F13;
    else if (name == L"F14") rc = ScanCode::F14;
    else if (name == L"F15") rc = ScanCode::F15;
    else if (name == L"F16") rc = ScanCode::F16;
    else if (name == L"F17") rc = ScanCode::F17;
    else if (name == L"F18") rc = ScanCode::F18;
    else if (name == L"F19") rc = ScanCode::F19;
    else if (name == L"F20") rc = ScanCode::F20;
    else if (name == L"F21") rc = ScanCode::F21;
    else if (name == L"F22") rc = ScanCode::F22;
    else if (name == L"F23") rc = ScanCode::F23;
    else if (name == L"F24") rc = ScanCode::F24;
    else if (name == L"NUMLOCK") rc = ScanCode::NUMLOCK;
    else if (name == L"SCROLL") rc = ScanCode::SCROLL;
    else if (name == L"OEM_NEC_EQUAL") rc = ScanCode::OEM_NEC_EQUAL;
    else if (name == L"OEM_FJ_MASSHOU") rc = ScanCode::OEM_FJ_MASSHOU;
    else if (name == L"OEM_FJ_TOUROKU") rc = ScanCode::OEM_FJ_TOUROKU;
    else if (name == L"OEM_FJ_LOYA") rc = ScanCode::OEM_FJ_LOYA;
    else if (name == L"OEM_FJ_ROYA") rc = ScanCode::OEM_FJ_ROYA;
    else if (name == L"LSHIFT") rc = ScanCode::LSHIFT;
    else if (name == L"RSHIFT") rc = ScanCode::RSHIFT;
    else if (name == L"LCONTROL") rc = ScanCode::LCONTROL;
    else if (name == L"RCONTROL") rc = ScanCode::RCONTROL;

    if (rc == -1) {
    if (name == L"LMENU") rc = ScanCode::LMENU;
    else if (name == L"RMENU") rc = ScanCode::RMENU;
    else if (name == L"BROWSER_BACK") rc = ScanCode::BROWSER_BACK;
    else if (name == L"BROWSER_FORWARD") rc = ScanCode::BROWSER_FORWARD;
    else if (name == L"BROWSER_REFRESH") rc = ScanCode::BROWSER_REFRESH;
    else if (name == L"BROWSER_STOP") rc = ScanCode::BROWSER_STOP;
    else if (name == L"BROWSER_SEARCH") rc = ScanCode::BROWSER_SEARCH;
    else if (name == L"BROWSER_FAVORITES") rc = ScanCode::BROWSER_FAVORITES;
    else if (name == L"BROWSER_HOME") rc = ScanCode::BROWSER_HOME;
    else if (name == L"VOLUME_MUTE") rc = ScanCode::VOLUME_MUTE;
    else if (name == L"VOLUME_DOWN") rc = ScanCode::VOLUME_DOWN;
    else if (name == L"VOLUME_UP") rc = ScanCode::VOLUME_UP;
    else if (name == L"MEDIA_NEXT_TRACK") rc = ScanCode::MEDIA_NEXT_TRACK;
    else if (name == L"MEDIA_PREV_TRACK") rc = ScanCode::MEDIA_PREV_TRACK;
    else if (name == L"MEDIA_STOP") rc = ScanCode::MEDIA_STOP;
    else if (name == L"MEDIA_PLAY_PAUSE") rc = ScanCode::MEDIA_PLAY_PAUSE;
    else if (name == L"LAUNCH_MAIL") rc = ScanCode::LAUNCH_MAIL;
    else if (name == L"LAUNCH_MEDIA_SELECT") rc = ScanCode::LAUNCH_MEDIA_SELECT;
    else if (name == L"LAUNCH_APP1") rc = ScanCode::LAUNCH_APP1;
    else if (name == L"LAUNCH_APP2") rc = ScanCode::LAUNCH_APP2;
    else if (name == L";") rc = ScanCode::OEM_1;
    else if (name == L"+") rc = ScanCode::OEM_PLUS;
    else if (name == L",") rc = ScanCode::OEM_COMMA;
    else if (name == L"-") rc = ScanCode::OEM_MINUS;
    else if (name == L".") rc = ScanCode::OEM_PERIOD;
    else if (name == L"/") rc = ScanCode::OEM_2;
    else if (name == L"~") rc = ScanCode::OEM_3;
    else if (name == L"[") rc = ScanCode::OEM_4;
    else if (name == L"\\") rc = ScanCode::OEM_5;
    else if (name == L"]") rc = ScanCode::OEM_6;
    }
    if (rc == -1)
    {
        if (name == L"\\") rc = ScanCode::OEM_5;
        else if (name == L"]") rc = ScanCode::OEM_6;
        else if (name == L"'") rc = ScanCode::OEM_7;
        else if (name == L"OEM_8") rc = ScanCode::OEM_8;
        else if (name == L"OEM_AX") rc = ScanCode::OEM_AX;
        else if (name == L"OEM_102") rc = ScanCode::OEM_102;
        else if (name == L"ICO_HELP") rc = ScanCode::ICO_HELP;
        else if (name == L"ICO_00") rc = ScanCode::ICO_00;
        else if (name == L"0") rc = ScanCode::ZERO;
        else if (name == L"1") rc = ScanCode::ONE;
        else if (name == L"2") rc = ScanCode::TWO;
        else if (name == L"3") rc = ScanCode::FREE;
        else if (name == L"4") rc = ScanCode::FOUR;
        else if (name == L"5") rc = ScanCode::FIVE;
        else if (name == L"6") rc = ScanCode::SIX;
        else if (name == L"7") rc = ScanCode::SEVEN;
        else if (name == L"8") rc = ScanCode::EIGHT;
        else if (name == L"9") rc = ScanCode::NINE;
        else if (name == L"A") rc = ScanCode::A;
        else if (name == L"B") rc = ScanCode::B;
        else if (name == L"C") rc = ScanCode::C;
        else if (name == L"D") rc = ScanCode::D;
        else if (name == L"E") rc = ScanCode::E;
        else if (name == L"F") rc = ScanCode::F;
        else if (name == L"G") rc = ScanCode::G;
        else if (name == L"H") rc = ScanCode::H;
        else if (name == L"I") rc = ScanCode::I;
        else if (name == L"J") rc = ScanCode::J;
        else if (name == L"K") rc = ScanCode::K;
        else if (name == L"L") rc = ScanCode::L;
        else if (name == L"M") rc = ScanCode::M;
        else if (name == L"N") rc = ScanCode::N;
        else if (name == L"O") rc = ScanCode::O;
        else if (name == L"P") rc = ScanCode::P;
        else if (name == L"Q") rc = ScanCode::Q;
        else if (name == L"R") rc = ScanCode::R;
        else if (name == L"S") rc = ScanCode::S;
        else if (name == L"T") rc = ScanCode::T;
        else if (name == L"U") rc = ScanCode::U;
        else if (name == L"V") rc = ScanCode::V;
        else if (name == L"W") rc = ScanCode::W;
        else if (name == L"X") rc = ScanCode::X;
        else if (name == L"Y") rc = ScanCode::Y;
        else if (name == L"Z") rc = ScanCode::Z;
    }
    return rc;
}

String ^ConsoleInput::keyCodeToName(int code)
{
    String ^rc;
 switch (code)
 {
    case ScanCode::LBUTTON: rc = L"LBUTTON"; break;
    case ScanCode::RBUTTON: rc = L"RBUTTON"; break;
    case ScanCode::CANCEL: rc = L"CANCEL"; break;
    case ScanCode::MBUTTON: rc = L"MBUTTON"; break;
    case ScanCode::XBUTTON1: rc = L"XBUTTON1"; break;
    case ScanCode::XBUTTON2: rc = L"XBUTTON2"; break;
    case ScanCode::BACK: rc = L"BACK"; break;
    case ScanCode::TAB: rc = L"TAB"; break;
    case ScanCode::CLEAR: rc = L"CLEAR"; break;
    case ScanCode::RETURN: rc = L"RETURN"; break;
    case ScanCode::SHIFT: rc = L"SHIFT"; break;
    case ScanCode::CONTROL: rc = L"CONTROL"; break;
    case ScanCode::MENU: rc = L"MENU"; break;
    case ScanCode::PAUSE: rc = L"PAUSE"; break;
    case ScanCode::CAPITAL: rc = L"CAPITAL"; break;
    case ScanCode::KANA: rc = L"KANA"; break;
    case ScanCode::JUNJA: rc = L"JUNJA"; break;
    case ScanCode::FINAL: rc = L"FINAL"; break;
    case ScanCode::HANJA: rc = L"HANJA"; break;
    case ScanCode::ESCAPE: rc = L"ESCAPE"; break;
    case ScanCode::CONVERT: rc = L"CONVERT"; break;
    case ScanCode::NONCONVERT: rc = L"NONCONVERT"; break;
    case ScanCode::ACCEPT: rc = L"ACCEPT"; break;
    case ScanCode::MODECHANGE: rc = L"MODECHANGE"; break;
    case ScanCode::SPACE: rc = L"SPACE"; break;
    case ScanCode::PRIOR: rc = L"PRIOR"; break;
    case ScanCode::NEXT: rc = L"NEXT"; break;
    case ScanCode::END: rc = L"END"; break;
    case ScanCode::HOME: rc = L"HOME"; break;
    case ScanCode::LEFT: rc = L"LEFT"; break;
    case ScanCode::UP: rc = L"UP"; break;
    case ScanCode::RIGHT: rc = L"RIGHT"; break;
    case ScanCode::DOWN: rc = L"DOWN"; break;
    case ScanCode::SELECT: rc = L"SELECT"; break;
    case ScanCode::PRINT: rc = L"PRINT"; break;
    case ScanCode::EXECUTE: rc = L"EXECUTE"; break;
    case ScanCode::SNAPSHOT: rc = L"SNAPSHOT"; break;
    case ScanCode::INSERT: rc = L"INSERT"; break;
    case ScanCode::DEL: rc = L"DEL"; break;
    case ScanCode::HELP: rc = L"HELP"; break;
    case ScanCode::LWIN: rc = L"LWIN"; break;
    case ScanCode::RWIN: rc = L"RWIN"; break;
    case ScanCode::APPS: rc = L"APPS"; break;
    case ScanCode::SLEEP: rc = L"SLEEP"; break;
    case ScanCode::NUMPAD0: rc = L"NUMPAD0"; break;
    case ScanCode::NUMPAD1: rc = L"NUMPAD1"; break;
    case ScanCode::NUMPAD2: rc = L"NUMPAD2"; break;
    case ScanCode::NUMPAD3: rc = L"NUMPAD3"; break;
    case ScanCode::NUMPAD4: rc = L"NUMPAD4"; break;
    case ScanCode::NUMPAD5: rc = L"NUMPAD5"; break;
    case ScanCode::NUMPAD6: rc = L"NUMPAD6"; break;
    case ScanCode::NUMPAD7: rc = L"NUMPAD7"; break;
    case ScanCode::NUMPAD8: rc = L"NUMPAD8"; break;
    case ScanCode::NUMPAD9: rc = L"NUMPAD9"; break;
    case ScanCode::MULTIPLY: rc = L"GRAY*"; break;
    case ScanCode::ADD: rc = L"GRAY+"; break;
    case ScanCode::SEPARATOR: rc = L"SEPARATOR"; break;
    case ScanCode::SUBTRACT: rc = L"GRAY-"; break;
    case ScanCode::DECIMAL: rc = L"GRAY."; break;
    case ScanCode::DIVIDE: rc = L"GRAY/"; break;
    case ScanCode::F1: rc = L"F1"; break;
    case ScanCode::F2: rc = L"F2"; break;
    case ScanCode::F3: rc = L"F3"; break;
    case ScanCode::F4: rc = L"F4"; break;
    case ScanCode::F5: rc = L"F5"; break;
    case ScanCode::F6: rc = L"F6"; break;
    case ScanCode::F7: rc = L"F7"; break;
    case ScanCode::F8: rc = L"F8"; break;
    case ScanCode::F9: rc = L"F9"; break;
    case ScanCode::F10: rc = L"F10"; break;
    case ScanCode::F11: rc = L"F11"; break;
    case ScanCode::F12: rc = L"F12"; break;
    case ScanCode::F13: rc = L"F13"; break;
    case ScanCode::F14: rc = L"F14"; break;
    case ScanCode::F15: rc = L"F15"; break;
    case ScanCode::F16: rc = L"F16"; break;
    case ScanCode::F17: rc = L"F17"; break;
    case ScanCode::F18: rc = L"F18"; break;
    case ScanCode::F19: rc = L"F19"; break;
    case ScanCode::F20: rc = L"F20"; break;
    case ScanCode::F21: rc = L"F21"; break;
    case ScanCode::F22: rc = L"F22"; break;
    case ScanCode::F23: rc = L"F23"; break;
    case ScanCode::F24: rc = L"F24"; break;
    case ScanCode::NUMLOCK: rc = L"NUMLOCK"; break;
    case ScanCode::SCROLL: rc = L"SCROLL"; break;
    case ScanCode::OEM_NEC_EQUAL: rc = L"OEM_NEC_EQUAL"; break;
    case ScanCode::OEM_FJ_MASSHOU: rc = L"OEM_FJ_MASSHOU"; break;
    case ScanCode::OEM_FJ_TOUROKU: rc = L"OEM_FJ_TOUROKU"; break;
    case ScanCode::OEM_FJ_LOYA: rc = L"OEM_FJ_LOYA"; break;
    case ScanCode::OEM_FJ_ROYA: rc = L"OEM_FJ_ROYA"; break;
    case ScanCode::LSHIFT: rc = L"LSHIFT"; break;
    case ScanCode::RSHIFT: rc = L"RSHIFT"; break;
    case ScanCode::LCONTROL: rc = L"LCONTROL"; break;
    case ScanCode::RCONTROL: rc = L"RCONTROL"; break;
    case ScanCode::LMENU: rc = L"LMENU"; break;
    case ScanCode::RMENU: rc = L"RMENU"; break;
    case ScanCode::BROWSER_BACK: rc = L"BROWSER_BACK"; break;
    case ScanCode::BROWSER_FORWARD: rc = L"BROWSER_FORWARD"; break;
    case ScanCode::BROWSER_REFRESH: rc = L"BROWSER_REFRESH"; break;
    case ScanCode::BROWSER_STOP: rc = L"BROWSER_STOP"; break;
    case ScanCode::BROWSER_SEARCH: rc = L"BROWSER_SEARCH"; break;
    case ScanCode::BROWSER_FAVORITES: rc = L"BROWSER_FAVORITES"; break;
    case ScanCode::BROWSER_HOME: rc = L"BROWSER_HOME"; break;
    case ScanCode::VOLUME_MUTE: rc = L"VOLUME_MUTE"; break;
    case ScanCode::VOLUME_DOWN: rc = L"VOLUME_DOWN"; break;
    case ScanCode::VOLUME_UP: rc = L"VOLUME_UP"; break;
    case ScanCode::MEDIA_NEXT_TRACK: rc = L"MEDIA_NEXT_TRACK"; break;
    case ScanCode::MEDIA_PREV_TRACK: rc = L"MEDIA_PREV_TRACK"; break;
    case ScanCode::MEDIA_STOP: rc = L"MEDIA_STOP"; break;
    case ScanCode::MEDIA_PLAY_PAUSE: rc = L"MEDIA_PLAY_PAUSE"; break;
    case ScanCode::LAUNCH_MAIL: rc = L"LAUNCH_MAIL"; break;
    case ScanCode::LAUNCH_MEDIA_SELECT: rc = L"LAUNCH_MEDIA_SELECT"; break;
    case ScanCode::LAUNCH_APP1: rc = L"LAUNCH_APP1"; break;
    case ScanCode::LAUNCH_APP2: rc = L"LAUNCH_APP2"; break;
    case ScanCode::OEM_1: rc = L";"; break;
    case ScanCode::OEM_PLUS: rc = L"+"; break;
    case ScanCode::OEM_COMMA: rc = L","; break;
    case ScanCode::OEM_MINUS: rc = L"-"; break;
    case ScanCode::OEM_PERIOD: rc = L"."; break;
    case ScanCode::OEM_2: rc = L"/"; break;
    case ScanCode::OEM_3: rc = L"~"; break;
    case ScanCode::OEM_4: rc = L"["; break;
    case ScanCode::OEM_5: rc = L"\\"; break;
    case ScanCode::OEM_6: rc = L"]"; break;
    case ScanCode::OEM_7: rc = L"'"; break;
    case ScanCode::OEM_8: rc = L"OEM_8"; break;
    case ScanCode::OEM_AX: rc = L"OEM_AX"; break;
    case ScanCode::OEM_102: rc = L"OEM_102"; break;
    case ScanCode::ICO_HELP: rc = L"ICO_HELP"; break;
    case ScanCode::ICO_00: rc = L"ICO_00"; break;
    case ScanCode::ZERO: rc = L"0"; break;
    case ScanCode::ONE: rc = L"1"; break;
    case ScanCode::TWO: rc = L"2"; break;
    case ScanCode::FREE: rc = L"3"; break;
    case ScanCode::FOUR: rc = L"4"; break;
    case ScanCode::FIVE: rc = L"5"; break;
    case ScanCode::SIX: rc = L"6"; break;
    case ScanCode::SEVEN: rc = L"7"; break;
    case ScanCode::EIGHT: rc = L"8"; break;
    case ScanCode::NINE: rc = L"9"; break;
    case ScanCode::A: rc = L"A"; break;
    case ScanCode::B: rc = L"B"; break;
    case ScanCode::C: rc = L"C"; break;
    case ScanCode::D: rc = L"D"; break;
    case ScanCode::E: rc = L"E"; break;
    case ScanCode::F: rc = L"F"; break;
    case ScanCode::G: rc = L"G"; break;
    case ScanCode::H: rc = L"H"; break;
    case ScanCode::I: rc = L"I"; break;
    case ScanCode::J: rc = L"J"; break;
    case ScanCode::K: rc = L"K"; break;
    case ScanCode::L: rc = L"L"; break;
    case ScanCode::M: rc = L"M"; break;
    case ScanCode::N: rc = L"N"; break;
    case ScanCode::O: rc = L"O"; break;
    case ScanCode::P: rc = L"P"; break;
    case ScanCode::Q: rc = L"Q"; break;
    case ScanCode::R: rc = L"R"; break;
    case ScanCode::S: rc = L"S"; break;
    case ScanCode::T: rc = L"T"; break;
    case ScanCode::U: rc = L"U"; break;
    case ScanCode::V: rc = L"V"; break;
    case ScanCode::W: rc = L"W"; break;
    case ScanCode::X: rc = L"X"; break;
    case ScanCode::Y: rc = L"Y"; break;
    case ScanCode::Z: rc = L"Z"; break;
    default:
        rc = L"UNKNOWN";
        break;
  }
  return rc;
}



}
}
}
