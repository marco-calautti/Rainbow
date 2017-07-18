//Copyright (C) 2014+ Marco (Phoenix) Calautti.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, version 2.0.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License 2.0 for more details.

//A copy of the GPL 2.0 should have been included with the program.
//If not, see http://www.gnu.org/licenses/

//Official repository and contact information can be found at
//http://github.com/marco-calautti/Rainbow

using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Rainbow.App.GUI.Model
{
    public class ListViewItemComparer : IComparer
    {
        public int ColumnIndex
        {
            get;
            set;
        }
        public SortOrder SortOrder
        {
            get;
            set;
        }
        public int Compare(object x, object y)
        {
            if (SortOrder == SortOrder.None)
            {
                return 0;
            }

            if (SortOrder == SortOrder.Descending)
            {
                var temp = x;
                x = y; y = temp;
            }
            
            var column1=((ListViewItem)x).SubItems[ColumnIndex];
            var column2=((ListViewItem)y).SubItems[ColumnIndex];

            if (ColumnIndex == 1)
            {
                return column1.Text.CompareTo(column2.Text);
            }

            return int.Parse(column1.Text).CompareTo(int.Parse(column2.Text));
        }
    }
}
