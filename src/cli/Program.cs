using gameEngine;
using tui;

var ui = new UI();
var eb = new EncounterBuilder();
var gameState = eb.makeEncounter("", ui);
gameState.Run();