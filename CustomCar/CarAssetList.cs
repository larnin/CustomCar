using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomCar
{
    class CarAssetList
    {
        static CarAssetList m_instance = new CarAssetList();
        public static CarAssetList instance { get { return m_instance; } }

        List<CarAsset> m_assets;

        void Initialize()
        {

        }

        CarAsset GetAsset(GameObject obj)
        {
            var localCar = obj.GetComponentInParent<LocalPlayerControlledCar>();
            if (localCar == null)
                return null;


            var profileManager = G.Sys.ProfileManager_;
            //profileManager.GetCarIndex


            return null;
        }
    }
}
