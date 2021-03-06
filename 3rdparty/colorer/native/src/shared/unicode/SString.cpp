
#include<stdio.h>
#include<unicode/String.h>

void SString::construct(const String *cstring, int s, int l){
  if (s < 0 || s > cstring->length() || l < -1) throw Exception(DString("bad string constructor parameters"));
  if (l == -1) l = cstring->length()-s;
  wstr = new wchar[l];
  for(len = 0; len < l; len++)
    wstr[len] = (*cstring)[s+len];
}

SString::SString(const String *cstring, int s, int l){
  construct(cstring, s, l);
}
SString::SString(const String &cstring, int s, int l){
  construct(&cstring, s, l);
}
SString::SString(char *str, int enc){
  DString ds(str, 0, -1, enc);
  construct(&ds, 0, ds.length());
}
SString::SString(wchar *str){
   DString ds(str, 0, -1);
   construct(&ds, 0, ds.length());
}
SString::SString(const wchar *str, int pos, int length){
   DString ds(str, pos, length);
   construct(&ds, 0, ds.length());
}
SString::SString(int no){
  char text[40];
  sprintf(text, "%d", no);
  construct(&DString(text), 0, -1);
}
SString::SString(){
  wstr = null;
  len = 0;
}
SString::~SString(){
  if(wstr) delete[] wstr;
}

wchar SString::operator[](int i) const{
  if (i >= len) throw StringIndexOutOfBoundsException(SString(i));
  return wstr[i];
}
int SString::length() const{
  return len;
}

/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is the Colorer Library.
 *
 * The Initial Developer of the Original Code is
 * Cail Lomecb <cail@nm.ru>.
 * Portions created by the Initial Developer are Copyright (C) 1999-2005
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */
