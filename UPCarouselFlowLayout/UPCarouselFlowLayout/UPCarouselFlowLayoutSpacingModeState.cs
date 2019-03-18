using System;

namespace UPCarouselFlowLayout
{
    internal class UPCarouselFlowLayoutSpacingModeState
    {
        public UPCarouselFlowLayoutSpacingMode Mode { get; }
        public nfloat Size { get; }

        public UPCarouselFlowLayoutSpacingModeState(UPCarouselFlowLayoutSpacingMode mode, nfloat size)
        {
            Mode = mode;
            Size = size;
        }
    }
}