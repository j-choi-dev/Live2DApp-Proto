using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LiveApp.Util
{
    public static class IDUtil
    {
        public static string GenerateUniqueId( List<string> list, string id )
        {
            if ( !list.Contains( id ) )
            {
                return id;
            }

            int count = 1;
            string newId;

            do
            {
                newId = $"{id}_{count}";
                count++;
            } while ( list.Contains( newId ) );

            return newId;
        }
    }
}
