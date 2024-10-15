using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Managers
{
    public class GameStarterManager : LoaderBase<DragManager>
    {
        private bool _isGameLoaded = false;
        public override void Init()
        {


            LoaderManager.OnEverythingLoaded += AllowInteraction;
            isLoaded = true;
        }

        private void OnEnable()
        {
            

        }

        private void OnDisable()
        {
            
        }

        void AllowInteraction()
        {
            _isGameLoaded = true;
            LoaderManager.OnEverythingLoaded -= AllowInteraction;
        }
    }
}
