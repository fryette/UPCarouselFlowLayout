namespace UPCarouselFlowLayout.Sample
{
    internal struct Character
    {
        public Character(string imageName, string name, string movie)
        {
            ImageName = imageName;
            Name = name;
            Movie = movie;
        }

        public string ImageName { get; }
        public string Name { get; }
        public string Movie { get; }
    }
}