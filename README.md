# Pacman-AI
<p><b>Author: Matas Gumbinas</b></p>
<p><b>Date: 2017</b></p>

<b>DESCRIPTION:</b>

Pacman game (AI vs AI) made on Unity game engine for university project.

Pacman uses ExpectiMinMax while some ghosts use MinMax algorithm while others may move randomly. 
The goal of pacman is to get as much points as possible by eating candies.
The ghosts on the other hand, tries to catch pacman as efficiently as possible with given MinMax settings (depth of search tree).

Pacman can also use Bayes Classificator that will allow it to dettermine which ghost moves depending on which algorithm (Random or MinMax).
Having this information, pacman can set appropriate values to its ExpectiMinMax tree nodes that allows pacman to outplay it's adversaries.

<b>BUGS:</b>

<ul>
  <li>Due to bad optimisation and inproper Unity game engine use during this project this program is very slow and stalls GUI;</li>
  <li>In some cases, heuristics seems to be inaccurate;</li>
</ul>
