using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class NameGen {

    private int order = 2;
    public int maxLen = 25;
    public int wordMaxLen = 10;
    public string maleFirstNames;
    public string worldNames;
    public string regionNames;
    

    private Dictionary<string, List<char>> worldNameTable;
    private Dictionary<string, List<char>> regionNameTable;
    private Dictionary<string, List<char>> maleFirstNameTable;

    // Use this for initialization
    public NameGen() {

        maleFirstNames = GetMaleFirstNames();
        worldNames = GetWorldNames();
        regionNames = GetRegionNames();

        worldNameTable = Load(worldNames, order);
        regionNameTable = Load(regionNames, order);
        maleFirstNameTable = Load(maleFirstNames, order);

    }

    public string GenerateWorldName(string seed = null)
    {
        if (seed == null || seed == "")
            seed = Time.time.ToString();
        return GenerateName(worldNameTable,seed);
    }
    public string GenerateRegionName(string seed = null)
    {
        if (seed == null || seed == "")
            seed = Time.time.ToString();
        return GenerateName(regionNameTable,seed);
    }
    public string GenerateMaleFirstName(string seed = null)
    {
        if (seed == null || seed == "")
            seed = Time.time.ToString();
        return GenerateName(maleFirstNameTable,seed);
    }
    /// <summary>
    /// Loads a string into a Markov chain dictionary
    /// </summary>
    /// <param name="table">the dictionary</param>
    /// <param name="names">the string of names</param>
    /// <param name="ord">The order of the Markov Chain</param>
    /// <param name="sep">Array of seperators for names string</param>
    /// <returns></returns>
    private Dictionary<string, List<char>> Load(string names, int ord, string[] sep = null)
    {
        if (sep == null)
        {
            sep = new string[] { ", ", ",", "/n", "\n" };
        }
        Dictionary<string, List<char>> table = new Dictionary<string, List<char>>();
        string[] namesAr = names.Split(sep, StringSplitOptions.RemoveEmptyEntries);
        order = ord;

        foreach (string s in namesAr)
        {
            for (int i = 0; i < s.Length - ord; i++)
            {
                string tableKey = s.Substring(i,ord);
                try
                {
                    table[tableKey].Add(s[i + ord]);
                }
                catch (KeyNotFoundException)
                {
                    table[tableKey] = new List<char>();
                    table[tableKey].Add(s[i + ord]);
                }
                
            }
        }

        return table;
        
    }


    private string GenerateName(Dictionary<string, List<char>> table, string seed = null, string start = null)
    {
        int max = wordMaxLen;
        string s = start;
        if (start == null)
        {
            s = randomStartChoice(table.Keys, seed);
        }

        try
        {
            while (s.Length < max && s.Length < maxLen)
            {
                string lastSlice = s.Substring(s.Length - order, order);
                char n = randomChoice(table[lastSlice]);
                string space = " ";
                string nString = n.ToString();
                if ( nString == space){
                    max += wordMaxLen;
                }
                s += randomChoice(table[lastSlice]);
            }
        }
        catch (KeyNotFoundException) { }

        return s;
            
    }

    private string randomStartChoice(Dictionary<string, List<char>>.KeyCollection keys, string seed = null)
    {
        
        List<string> keyList = new List<string>();
        foreach (string key in keys)
        {
            if (Char.IsUpper(key[0]))
                keyList.Add(key);
        }

        string selectedKey = "" ;

        if (seed == null)
            seed = Time.time.ToString();

        System.Random randNum = new System.Random(seed.GetHashCode());
        int actualNum = randNum.Next(keyList.Count);
        selectedKey = keyList[actualNum];

        return selectedKey;
    }
    private char randomChoice(List<char> charList)
    {
        char selectedChar = new Char();
        int randNum = UnityEngine.Random.Range(0, charList.Count);
        selectedChar = charList[randNum];

        return selectedChar;
    }

    private string GetMaleFirstNames()
    {
        string names = "Bob, Matthew, Chinedu, Daniel, Emanuel, Aaron, Jonathan";
        return names;
    }
    private string GetRegionNames()
    {
        string names = GetWorldNames();
        return names;
    }

    private string GetWorldNames()
    {
        string names = @"Yggdrasill
Flint
Granite
Quartz
Vine
Sap
Ground
Bane
Echo
Iron
Steel
Mud
Flower
Meld
Petra
Salt
Geode
Mold
Crystal
Forge
Fever
Corona
Scorch
Ember
Flash
Torch
Cannon
Spark
Kindle
Char
Coal
Reflux
Core
Tinder
Shine
Fury
Fugue
Gust
Breeze
Zephyr
Smog
Kite
Squall
Luff
Breath
Blitz
Ether
Waft
Haze
Wheeze
Aroma
Whorl
Gasp
Lull
Gale
Fizz
Sleet
Mist
Spritz
Hail
Tonic
Dew
Fog
Sour
spring
Shade
Chill
Steam
Rime
Gel
Eddy
Balm
Serac
Venus
Ramses
Cybelle
Judgement
Mars
Kirin
Tiamat
Meteor
Mercury
Nereid
Neptune
Boreas
Jupiter
Atalanta
Procne
Thor
Zagan
Meagaera
Flora
Moloch
Ulysses
Eclipse
Haures
Coatlicue
Daedalus
Azul
Catastrophe
Charon
Iris
Ruffian
Punch Ant
Giant Bat
Wild Wolf
Mimic
Angle Worm
Amaze
Chestbeater
Ghost
Skeleton
Mini-Goblin
Rat Soldier
Troll
Will Head
Rat
Drone Bee
Kobold
Dino
Momonga
Emu
Spider
King Scorpion
Gnome
Briggs
Sea Fighter
Ooze
Harpy
Ghoul
Creeper
Mummy
Wolfkin Cub
Wyvern chick
Flash Ant
Wild Gorilla
Bone Fighter
Death Head
Mad Mole
Pixie
Dirge
Doomsayer
Salamander
Spirit
Red Demon
Mad Plant
Bandit
Thief
Aqua Jelly
Aqua Hydra
Seabird
Urchin Beast
Fighter Bee
Calamar
Merman
Numb Ant
Death Cap
Alec Goblin
Mad Vermin
Dire Wolf
Undead
Ravager
Ghost Mage
Faery
Cave Troll
Man o' War
Roc
Virago
Lizard Fighter
Sea Dragon
Needle Egg
Squirrelfang
Dinox
Minotaurus
Living Armor
Harridan
Stone soldier
Magicore
Wight
Fire Worm
Clay Gargoyle
Golem
Gnome Mage
Dread Hound
Wood Walker
Serpent
Gressil
Avimander
Poseidon
Hydra
Gillman
Seafowl
Vile Dirge
Spiral Shell
Turtle Dragon
Wolfkin
Wargold
Slayer
Pteranodon
Talon Runner
Wild Gryphon
Navampa
Azart
Satrage
Moapa
Knight
Nightmare
Mole Mage
Wyvern
Foul Mummy
Devil Scorpion
Macetail
Ghost Army
Blue Dragon
Karst
Agatio
Raging Rock
Lich
Little Death
Phoenix
Wise Gryphon
Lesser Demon
Grand Chimera
Raptor
Doodle Bug
Minos Warrior
Devil Frog
Fire Bird
Aka Manah
Flame Dragon
Doom Dragon
Minos Knight
Sand Scorpion
Winged Lizard
Soul Army
Fire Dragon
Valukar
Earth Golem
Cannibal Ghoul
Pyrodra
Great Seagull
Ocean Dragon
Sea Hedgehog
Puppet Warrior
Gillman Lord
Refresh Ball
Guardian Ball
Thunder Ball
Anger Ball
Star Magician
Chimera Worm
Druj
Wonder Bird
Cruel Dragon
Sentinel
Bombander
Sky Dragon
Mad Demon
Grave Wright
Dullahan
Torent
Orcrot
Marcroid
Minicoid
Tentacle Plant
Mocking Plant
Mandragora
Alraune
Insect Plant
Carnivorous Plant
Bomb Plant
Bomb Seedling
Pumpkin Tree
Bellpepper Head
Boxer Iris
Orchid
Poison Lilly
Wolf
Night Raid
Bear
Egg Bear
Rabbit
Hare
Bigfoot
Sidewinder
Violent Viper
Manticore
Chimaera
Lobo
Sasquatch
Boar
Baby Boar
Basilisk
Sewer Rat
ArmaBoar
Zombie
Ghoul
Demon
Arch Demon
Skeleton
Gold Skeleton
Undertaker
Coffinmaster
Living Armor
Specter
Phantasm
Death
Grim Reaper
Ghost
Phantom
Lamia
Medusa
Doom Guard
Phantom Knight
Hell Knight
Samael
Pharaoh Knight
Golem
Rock Golem
Clay Golem
Gentleman
Living doll
Teddy
Living Sword
Melting Pot
Brown Pot
Fire element
Gargoyle
Neviros
Ice Warrior
Fire Warrior
Thunder Sword
Fake
Water Element
Wind Element
Earth Element
Hammer Knuckle
Murder
Perfect Murder
Raybit
Cybit
Thief
Rogue
Soldier
Duelist
Warrior
Heavy Armor
Dragon Rider
Archer
Ranger
Witch
Sorceress
Sorcerer
Druid
Ogre
Beast Ogre
Whip Master
Bowman
Spearman
Foot Soldier
Commander
Cardinal Knight
Commander Knight
Warrior
Convict
Sorcerer
Angel Spearman
Angel Swordian
Angel Commander
Angel Archer
Hawk
Storm Claw
Axe Beak
Dodo
Harpy
Feather Magic
Fire Bird
Lightning Bird
Penguinist
Penguiner
Black Bat
Cockatrice
Red Bat
Giant Bee
Killer Bee
Scorpion
Scarlet Needle
Woods Worm
Tropical Worm
Sand Worm
Sliver
Mantis
Red Mantis
Spider
Arachnid
Giant Beetle
Gold Beetle
Grasshopper
Ice Spider
Deathseeker
Starfish
Super Star
Tortoise
Crush Tortoise
Octoslime
Kraaken
Fish
Seaspin
Float Dragon
Seahorse
Jellyfish
Sea Jelly
Mermaid
Jelly
Sea Dragon
Sea Horror
Slime
Gold Slime
Giant Leech
Giant Slug
Roller Snail
Giant snail
Green Roper
Red Roper
Bacura
Cutlass
Cave Worm
Man-eater
Sheldra
Spiked Snail
Wyvern
Drake
Dragon
Gold Dragon
Dark Dragon
Dragon Knight
Velocidragon
Exbelua
Windmaster
Ktugach
Ktugachling
Adulocia
Amphitra
Lapyx
Lubaris
Kilia
Winged Dragon
Baby Dragon
Guardian Wind
Guardian Lightning
Sword Dancer
Fenrir
Idun
Rodyle
Undine
Gnome
Efreet
Volt
Celsius
Luna
Aska
Shadow
Maxwell
Origin
Sephie
Yutis
Fairess
The Fugutive
The Neglected
The Judged
Defense System
Orbit
Guard Arm
Auto Repair Unit
Kratos Aurion
Magnius
Kvar
Energy Stone
Vidarr
Forcystus
Exbone
Pronyma
Kuchinawa
Botta
Seles
Garr
Farah Oersted
Meredy
Abyssion
Zelos Wilder
Mithos
Yuan
Remiel
Gatekeeper
Plantix
Dark Spear
Dark Sword
Dark Commander
Dark Archer";
        return names;
    }

}

