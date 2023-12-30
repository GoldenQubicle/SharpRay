

using System.Net.WebSockets;
using AoC;
using Common;

const int height = 720;
const int width = 720;
const int cells = 72;

var config = new SharpRayConfig
{
	WindowWidth = width,
	WindowHeight = height
};

Initialize(config);

var grid = new Grid2d(new PfSolution("day18test1").Input);


var image = GenImageChecked(width, height, width/grid.Width, height/grid.Height, Color.VIOLET, Color.DARKPURPLE);
var texture = new ImageTexture(image, Color.WHITE);

AddEntity(texture);

Run();


