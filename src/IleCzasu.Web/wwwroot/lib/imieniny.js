/*
 * Nameday   ver  2.0.1  2003-11-02
 * Copyright (c) 2002-2003 by Michal Nazarewicz (mina86@tlen.pl)
 *
 * This script is free software; It is ditributed under terms of
 * GNU Lesser General Public License. Copy of the license can be found
 * at www.gnu.org/licenses/licenses.html#LGPL
 *
 * Visit www.projektcode.prv.pl for more..
 */


//
// Tuday's date :)
//
var nameday_date = new Date(),
	nameday_day = nameday_date.getDate(),
	nameday_month = nameday_date.getMonth()+1;



//
// Object representing names
//
function NamedayNames(names) {
	if (names instanceof Array) {
		this.names = names;
	} else {
		this.names = names.split('|');
	}
}

NamedayNames.prototype = {
	join: function(sep, last_sep, limit) {
		// Init args
		switch (arguments.length) {
			case  0: sep = null;
			case  1: last_sep = null;
			case  2: limit = null;
			case  3: break;
			default: return false;
		}


		// Get names
		var names = this.getNames(2);


		// Join
		if (sep==null) {
			sep = ', ';
		}
		if (last_sep==null) {
			return names.join(sep);
		} else {
			var str = '';
			for (var i = 0; i<names.length; i++) {
				if (i==names.length-1) {
					str += last_sep;
				} else if (i) {
					str += sep;
				}
				str += names[i];
			}
			return str;
		}
	},


	//
	// Returns names as formated string
	//
	toString: function(before, after, sep, last_sep, limit) {
		// Init args
		switch (arguments.length) {
			case  0: before = null;
			case  1: after = null;
			case  2: sep = null;
			case  3: last_sep = null;
			case  4: limit = null;
			case  5: break;
			default: return false;
		}


		// Join names
		var str = this.join(sep, last_sep, limit);
		if (!str) {
			return false;
		}


		// Return
		return (before==null?'':before) + str + (after==null?'':after);
	},


	//
	// Returns names in array (maximum number of names in array is limit
	// or there's no maximum number if limit==0 || limit==null)
	//
	getNames: function(limit) {
		// Check args;
		if (arguments.length>1) {
			return false;
		}

		// All requested
		if (arguments.length==0 || limit==null || limit<1 ||
			limit>=this.names.length) {
			return this.names;

		// Limit requested
		} else {
			var arr = new Array(limit);
			for (var i = 0; i<limit; i++) {
				arr[i] = this.names[i];
			}
			return arr;
		}
	},


	//
	// Get name at index
	//
	get: function(index) {
		return this.names[index];
	},


	//
	// Get number of names
	//
	count: function() {
		return this.names.length;
	}
};



//
// Object representing set of names for each day of year
//
function NamedaySet(array) {
	this.array = array;
}

NamedaySet.prototype = {
	//
	// Returns NamedayNames object with names of people who have nameday
	// today or in the dth of m  If d or m is null or omitted, todays day
	// and/or month is taken.
	// Note: Months are indexed from 1 !!
	//
	getNames: function(d, m) {
		switch (arguments.length) {
			case  0: d = null;
			case  1: m = null;
			case  2: break;
			default: return false;
		}

		if (d==null) {
			d = nameday_day;
		}
		if (m==null) {
			m = nameday_month;
		}

		return new NamedayNames(this.array[m-1][d-1]);
	}
};




//
// Main object
//
function Nameday() {
	this.sets = new Array();
}


Nameday.prototype = {
	//
	// Returns specyfied set
	//
	getSet: function(lang) {
		if (arguments.length!=1) {
			return false;
		}
		return this.sets['' + lang];
	},


	//
	// Adds set
	//
	addSet: function(lang, set) {
		if (arguments.length!=2) {
			return false;
		}
		if (set instanceof NamedaySet) {
			this.sets['' + lang] = set;
		} else {
			this.sets['' + lang] = new NamedaySet(set);
		}
	}
};

var nameday = new Nameday();



/*
 * Nameday Polish Extension  ver  1.4.2  2003-11-19
 * Copyright (c) 2002-2003 by Michal Nazarewicz (mina86@tlen.pl)
 *
 * This script is free software; It is ditributed under terms of
 * GNU Lesser General Public License. Copy of the license can be found
 * at www.gnu.org/licenses/licenses.html#LGPL
 */


//
// Converts names
//
NamedayNames.prototype.pl_convert = function(method) {
	if (arguments.length!=1) {
		return false;
	}
	if (method==0) {
		return new NamedayNames(this.names);
	}
	if (method!=1) {
		return false;
	}

	var ret = new Array(), name = '';
	for (var i = 0; i<this.names.length; i++) {
		name = this.names[i];

		var len = name.length,
			last3 = name.substring(len-3),
			last2 = name.substring(len-2),
			vowel3 = "aeioóuy".indexOf(name.charAt(len-4))!=-1,
			vowel2 = "aeioóuy".indexOf(name.charAt(len-3))!=-1;

		if (last3=="ego") {
			if (name.substring(len-4, 1)=='l') {
				name = name.substring(0, len-3);
			} else {
				name = name.substring(0, len-3) + "y";
			}
		} else if (last3=="ńca") {
			name = name.substring(0, len-3) + "niec";
		} else if (last3=="tra") {
			name = name.substring(0,len-3) + (vowel3?"tr":"ter");
		} else if (last2=="ka" && !vowel2) {
			name =  name.substring(0,len-2) + "ek";
		} else if (last2=="ła" && !vowel2) {
			name = name.substring(0, len-2) + "ła";
		} else {
			name = name.substring(0, len-1) +
				(last2.substring(2,1)=='a'?'':'a');
		}

		ret[i] = name;
	}
	return new NamedayNames(ret);
};


//
// For backward compatibility
//
function WypiszImieniny(before, after, sep, last_sep, method) {
    switch (arguments.length) {
        case 0: before = "";
        case 1: after = "";
        case 2: sep = null;
        case 3: last_sep = null;
        case 3: method = null;
    }


    var names = PobierzImieniny(sep, last_sep, method);




    return ("" + before + names + after);
}

function PobierzImieniny(sep, last_sep, method) {
	switch (arguments.length) {
		case 0: sep = null;
		case 1: last_sep = null;
		case 2: method = null;
	}
	if (method==null) {
		method = 0;
	}

	var names;
	if (!(names = nameday.getSet('pl')) || !(names = names.getNames()) ||
		!(names = names.pl_convert(method))) {
		return false;
	}

	return names.toString('', '', sep, last_sep);
}



/*
 * Nameday Polish Names Database  v 2.1
 * Database taken from infoludek.pl/~slawek/imieniny.html
 * +some corrections
 */


nameday.addSet('pl', new Array(
	new Array(
		"Masława|Mieczysława|Mieszka",
		"Bazylego|Makarego|Narcyzy",
		"Arlety|Danuty|Lucjana",
		"Anieli|Elżbiety|Tytusa",
		"Edwarda|Hanny|Szymona",
		"Kacpra|Melchiora|Baltazara",
		"Juliana|Lucjana|Walentyny",
		"Artura|Rajmunda|Seweryny",
		"Adriana|Alicji|Teresy",
		"Ady|Jana|Wilhelma",
		"Feliksa|Honoraty|Marty",
		"Bernarda|Czesławy|Grety",
		"Bogumiła|Bogumiły|Weroniki",
		"Feliksa|Hilarego|Martyny",
		"Arnolda|Dory|Pawła",
		"Marcelego|Walerii|Włodzimierza",
		"Antoniego|Henryki|Mariana",
		"Beatrycze|Małgorzaty|Piotra",
		"Erwiny|Henryka|Mariusza",
		"Fabioli|Miły|Sebastiana",
		"Agnieszki|Jarosława|Nory",
		"Dominiki|Mateusza|Wincentego",
		"Fernandy|Jana|Rajmundy",
		"Felicji|Roberta|Sławy",
		"Miłosza|Pawła|Tatiany",
		"Lutosława|Normy|Pauliny",
		"Anieli|Juliana|Przemysława",
		"Agnieszki|Kariny|Lesława",
		"Franciszka|Konstancji|Salomei",
		"Martyny|Macieja|Teofila",
		"Joanny|Ksawerego|Luizy"
	),
	new Array(
		"Brygidy|Dobrogniewa|Ignacego",
		"Kornela|Marii|Mirosławy",
		"Błażeja|Joanny|Telimeny",
		"Andrzeja|Mariusza|Weroniki",
		"Agaty|Filipa|Justyniana",
		"Amandy|Bogdana|Doroty",
		"Ryszarda|Teodora|Wilhelminy",
		"Irminy|Piotra|Sylwii",
		"Bernarda|Eryki|Rajmunda",
		"Elwiry|Elizy|Jacka",
		"Bernadetty|Marii|Olgierda",
		"Czasława|Damiana|Normy",
		"Grzegorza|Lesława|Katarzyny",
		"Liliany|Walentyny|Walentego",
		"Arnolda|Jowity|Georginy",
		"Danuty|Daniela|Juliany",
		"Donata|Gizeli|Łukasza",
		"Konstancji|Krystiana|Sylwany",
		"Bettiny|Konrada|Mirosława",
		"Anety|Lecha|Leona",
		"Eleonory|Lenki|Kiejstuta",
		"Małgorzaty|Marty|Nikifora",
		"Damiana|Romana|Romany",
		"Boguty|Bogusza|Macieja",
		"Almy|Cezarego|Jarosława",
		"Bogumiła|Eweliny|Mirosława",
		"Gagrieli|Liwii|Leonarda",
		"Ludomira|Makarego|Wiliany",
		"Lecha|Lutomira|Wiktora"
	),
	new Array(
		"Albina|Antoniny|Radosławy",
		"Halszki|Heleny|Karola",
		"Kingi|Maryna|Tycjana",
		"Adrianny|Kazimierza|Wacława",
		"Aurory|Fryderyka|Oliwii",
		"Jordana|Marcina|Róży",
		"Flicyty|Kajetana|Pauli",
		"Beaty|Juliana|Wincentego",
		"Dominika|Franciszki|Katarzyny",
		"Bożysławy|Cypriana|Marcelego",
		"Konstantego|Ludosława|Rozyny",
		"Grzegorza|Justyny|Józefiny",
		"Bożeny|Krystyny|Marka",
		"Dalii|Leona|Matyldy",
		"Delfiny|Longina|Ludwiki",
		"Izabeli|Henryka|Oktawii",
		"Reginy|Patryka|Zdyszka",
		"Edwarda|Narcyza|Zbysława",
		"Aleksandryny|Józefa|Nicety",
		"Joachima|Kiry|Maurycego",
		"Benedykta|Lubomiry|Lubomira",
		"Bogusława|Jagody|Katarzyny",
		"Feliksa|Konrada|Zbysławy",
		"Gabrieli|Marka|Seweryna",
		"Bolka|Cezaryny|Marioli",
		"Dory|Olgi|Teodora",
		"Ernesta|Jana|Marka",
		"Anieli|Kasrota|Soni",
		"Marka|Wiktoryny|Zenona",
		"Amelii|Dobromira|Leonarda",
		"Balbiny|Kamila|Kornelii"
	),
	new Array(
		"Chryzamtyny|Grażyny|Zygmunta",
		"Franciszka|Malwiny|Władysława",
		"Pankracego|Renaty|Ryszarda",
		"Benedykta|Izodory|Wacławy",
		"Ireny|Kleofasa|Wincentego",
		"Ady|Celestyny|Ireneusza",
		"Donata|Herminy|Rufina",
		"Amadeusza|Cezaryny|Juliany",
		"Mai|Marcelego|Wadima",
		"Borysławy|makarego|Michała",
		"Filipa|Izoldy|Leona",
		"Juliusza|Lubosława|Wiktoryny",
		"Artemona|Justyny|Przemysławy",
		"Bernarda|Martyny|Waleriana",
		"Adolfiny|Odetty|Wacława",
		"Bernarda|Biruty|Erwina",
		"Anicety|Klary|Rudolfina",
		"Apoloniusza|Bogusławy|Gocisławy",
		"Alfa|Leonii|Tytusa",
		"Agnieszki|Amalii|Czecha",
		"Jarosława|Konrada|Selmy",
		"Łukasza|Kai|Nastazji",
		"Ilony|Jerzego|Wojciecha",
		"Bony|Horacji|Jerzego",
		"Jarosława|Marka|Wiki",
		"Marii|Marzeny|Ryszarda",
		"Sergiusza|Teofila|Zyty",
		"Bogny|Walerii|Witalisa",
		"Hugona|Piotra|Roberty",
		"Balladyny|Lilli|Mariana"
	),
	new Array(
		"Józefa|Lubomira|Ramony",
		"Longiny|Toli|Zygmunta",
		"Jaropełka|Marii|Niny",
		"Floriana|Michała|Moniki",
		"Irydy|Tamary|Waldemara",
		"Beniny|Filipa|Judyty",
		"Augusta|Gizeli|Ludomiry",
		"Kornela|Lizy|Stanisława",
		"Grzegorza|Karoliny|Karola",
		"Antoniny|Izydory|Jana",
		"Igi|Mamerta|Miry",
		"Dominika|Imeldy|Pankracego",
		"Agnieszki|Magdaleny|Serwacego",
		"Bonifacego|Julity|Macieja",
		"Dionizego|Nadziei|Zofii",
		"Andrzeja|Jędrzeja|Małgorzaty",
		"Brunony|Sławomira|Wery",
		"Alicji|Edwina|Eryka",
		"Celestyny|Iwony|Piotra",
		"Bazylego|Bernardyna|Krystyny",
		"Jana|Moniki|Wiktora",
		"Emila|Neleny|Romy",
		"Leoncjusza|Michała|Renaty",
		"Joanny|Zdenka|Zuzanny",
		"Borysa|Magdy|Marii-Magdaleny",
		"Eweliny|Jana|Pawła",
		"Amandy|Jana|Juliana",
		"Augustyna|Ingi|Jaromira",
		"Benity|Maksymiliana|Teodozji",
		"Ferdynanda|Gryzeldy|Zyndrama",
		"Anieli|Feliksa|Kamili"
	),
	new Array(
		"Gracji|Jakuba|Konrada",
		"Erazma|Marianny|Marzeny",
		"Anatola|Leszka|Tamary",
		"Christy|Helgi|Karola",
		"Bonifacego|Kiry|Waltera",
		"Laury|Laurentego|Nory",
		"Ariadny|Jarosława|Roberta",
		"Ady|Celii|Medarda",
		"Anny-Marii|Felicjana|Sławoja",
		"Bogumiła|Diany|Małgorzaty",
		"Barnaby|Benedykta|Flory",
		"Gwidona|Leonii|Niny",
		"Antoniego|Gracji|Lucjana",
		"Bazylego|Elizy|Justyny",
		"Jolanty|Lotara|Wita",
		"Aliny|Anety|Benona",
		"Laury|Leszka|Marcjana",
		"Elżbiety|Marka|Pauli",
		"Gerwazego|Protazego|Sylwii",
		"Bogny|Rafaeli|Rafała",
		"Alicji|Alojzego|Rudolfa",
		"Pauliny|Sabiny|Tomasza",
		"Albina|Wandy|Zenona",
		"Danuty|Jana|Janiny",
		"Łucji|Witolda|Wilhelma",
		"Jana|Pauliny|Rudolfiny",
		"Cypriana|Emanueli|Władysława",
		"Florentyny|Ligii|Leona",
		"Pawła|Piotra|Salomei",
		"Arnolda|Emiliany|Lucyny"
	),
	new Array(
		"Bogusza|Haliny|Mariana",
		"Kariny|Serafiny|Urbana",
		"Anatola|Jacka|Mirosławy",
		"Aureli|Malwiny|Zygfryda",
		"Antoniego|Bartłomieja|Karoliny",
		"Dominiki|Jaropełka|Łucji",
		"Estery|Kiry|Rudolfa",
		"Arnolda|Edgara|Elżbiety",
		"Hieronima|Palomy|Weroniki",
		"Filipa|Sylwany|Witalisa",
		"Benedykta|Kariny|Olgi",
		"Brunona|Jana|Wery",
		"Danieli|Irwina|Małgorzaty",
		"Kamili|Kamila|Marcelego",
		"Henryka|Igi|Włodzimierza",
		"Eustachego|Mariki|Mirelli",
		"Aleksego|Bogdana|Martyny",
		"Kamila|Karoliny|Roberta",
		"Alfreny|Rufina|Wincentego",
		"Fryderyka|Małgorzaty|Seweryny",
		"Danieli|Wawrzyńca|Wiktora",
		"Magdaleny|Mileny|Wawrzyńca",
		"Sławy|Sławosza|Żelisławy",
		"Kingi|Krystyna|Michaliny",
		"jakuba|Krzysztofa|Walentyny",
		"Anny|Mirosławy|Joachima",
		"Aureliusza|Natalii|Rudolfa",
		"Ady|Wiwiany|Sylwiusza",
		"Marty|Konstantego|Olafa",
		"Julity|Ludmiły|Zdobysława",
		"Ignacego|Lodomiry|Romana"
	),
	new Array(
		"Jarosława|Justyny|Nadziei",
		"Gustawa|Kariny|Stefana",
		"Augustyna|Kamelii|Lidii",
		"Dominiki|Dominika|Protazego",
		"Emila|Karoliny|Kary",
		"Jakuba|Sławy|Wincentego",
		"Donaty|Olechny|Kajetana",
		"Izy|Rajmunda|Seweryna",
		"Klary|Romana|Rozyny",
		"Bianki|Borysa|Wawrzyńca",
		"Luizy|Włodzmierza|Zuzanny",
		"Hilarii|Juliana|Lecha",
		"Elwiry|Hipolita|Radosławy",
		"Alfreda|Maksymiliana|Selmy",
		"Marii|Napoleona|Stelli",
		"Joachima|Nory|Stefana",
		"Anity|Elizy|Mirona",
		"Bogusława|Bronisława|Ilony",
		"Emilii|Julinana|Konstancji",
		"Bernarda|Sabiny|Samuela",
		"Franciszka|Kazimiery|Ruty",
		"Cezarego|Marii|Zygfryda",
		"Apolinarego|Miły|Róży",
		"Bartosza|Jerzego|Maliny",
		"Belii|Ludwika|Luizy",
		"Ireneusza|Konstantego|Marii",
		"Cezarego|Małgorzaty|Moniki",
		"Adeliny|Erazma|Sobiesława",
		"Beaty|Racibora|Sabiny",
		"Benona|Jowity|Szczęsnego",
		"Cyrusa|Izabeli|Rajmundy"
	),
	new Array(
		"Belindy|Bronisza|Idziego",
		"Dionizy|Izy|Juliana",
		"Joachima|Liliany|Szymona",
		"Dalii|Idy|Rocha",
		"Doroty|Justyna|Wawrzyńca",
		"Beaty|Eugeniusza|Lidy",
		"Reginy|Marka|Melchiora",
		"Czcibora|Marii|Radosława",
		"Aldony|Jakuba|Sergiusza",
		"Eligii|Irmy|Łukasza",
		"Dagny|Jacka|Prota",
		"Amadeusza|Gwidy|Sylwiny",
		"Apolinarego|Eugenii|Lubomira",
		"Bernarda|Mony|Roksany",
		"Albina|Lolity|Ronalda",
		"Jagienki|Kamili|Korneliusza",
		"Franciszka|Lamberty|Narcyza",
		"Ireny|Irminy|Stanisława",
		"Januarego|Konstancji|Leopolda",
		"Eustachego|Faustyny|Renaty",
		"Darii|Mateusza|Wawrzyńca",
		"Maury|Milany|Tomasza",
		"Bogusława|Liwiusza|Tekli",
		"Dory|Gerarda|Maryny",
		"Aureli|Kamila|Kleofasa",
		"Cypriana|Justyny|Łucji",
		"Damiana|Mirabeli|Wincentego",
		"Libuszy|Wacławy|Wacława",
		"Michaliny|Michała|Rafała",
		"Geraldy|Honoriusza|Wery"
	),
	new Array(
		"Heloizy|Igora|Remigiusza",
		"Racheli|Sławy|Teofila",
		"Bogumiła|Gerarda|Józefy",
		"Edwina|Rosławy|Rozalii",
		"Flawii|Justyna|Rajmunda",
		"Artura|Fryderyki|Petry",
		"Krystyna|Marii|Marka",
		"Brygidy|Loreny|Marcina",
		"Arnolda|Ludwika|Sybili",
		"Franciszka|Loretty|Poli",
		"Aldony|Brunona|Emila",
		"Krystyny|Maksa|Serafiny",
		"Edwarda|Geraldyny|Teofila",
		"Alany|Damiana|Liwii",
		"Jadwigi|Leonarda|Teresy",
		"Ambrożego|Florentyny|Gawła",
		"Antonii|Ignacego|Wiktora",
		"Hanny|Klementyny|Łukasza",
		"Michaliny|Michała|Piotra",
		"Ireny|Kleopatry|Witalisa",
		"Celiny|Hilarego|Janusza",
		"Haliszki|Lody|Przybysława",
		"Edwarda|Marleny|Seweryna",
		"Arety|Marty|Marcina",
		"Ingi|Maurycego|Sambora",
		"Ewarysta|Lucyny|Lutosławy",
		"Iwony|Noemi|Szymona",
		"Narcyza|Serafina|Wioletty",
		"Angeli|Przemysława|Zenobii",
		"Augustyny|Łukasza|Urbana",
		"Krzysztofa|Augusta|Saturnina"
	),
	new Array(
		"Konrada|Seweryny|Wiktoryny",
		"Bohdany|Henryka|Tobiasza",
		"Huberta|Miły|Sylwii",
		"Albertyny|Karola|Olgierda",
		"Balladyny|Elżbiety|Sławomira",
		"Arletty|Feliksa|Leonarda",
		"Antoniego|Kaliny|Przemiły",
		"Klaudii|Seweryna|Wiktoriusza",
		"Anatolii|Gracji|Teodora",
		"Leny|Lubomira|Natalii",
		"Bartłomieja|Gertrudy|Marcina",
		"Konrada|Renaty|Witolda",
		"Arkadii|Krystyna|Stanisławy",
		"Emila|Laury|Rogera",
		"Amielii|Idalii|Leopolda",
		"Edmunda|Marii|Marka",
		"Grzegorza|Salomei|Walerii",
		"Klaudyny|Romana|Tomasza",
		"Elżbiety|Faustyny|Pawła",
		"Anatola|Edyty|Rafała",
		"Janusza|Marii|Reginy",
		"Cecylii|Jonatana|Marka",
		"Adeli|Felicyty|Klemensa",
		"Emmy|Flory|Romana",
		"Elżbiety|Katarzyny|Klemensa",
		"Leona|Leonarda|Lesławy",
		"Franciszka|Kseni|Maksymiliana",
		"Jakuba|Stefana|Romy",
		"Błażeja|Margerity|Saturnina",
		"Andrzeja|Maury|Ondraszka"
	),
	new Array(
		"Blanki|Edmunda|Eligiusza",
		"Balbiny|Ksawerego|Pauliny",
		"Hilarego|Franciszki|Ksawery",
		"Barbary|Hieronima|Krystiana",
		"Kryspiny|Norberta|Sabiny",
		"Dionizji|Leontyny|Mikołaja",
		"Agaty|Dalii|Sobiesława",
		"Delfiny|Marii|Wirginiusza",
		"Anety|Leokadii|Wiesława",
		"Danieli|Bohdana|Julii",
		"Biny|Damazego|Waldemara",
		"Ady|Aleksandra|Dagmary",
		"Dalidy|Juliusza|Łucji",
		"Alfreda|Izydora|Zoriny",
		"Celiny|Ireneusza|Niny",
		"Albiny|Sebastiana|Zdzisławy",
		"Jolanty|Łukasza|Olimpii",
		"Bogusława|Gracjana|Laury",
		"Beniaminy|Dariusza|Gabrieli",
		"Bogumiły|Dominika|Zefiryna",
		"Honoraty|Seweryny|Tomasza",
		"Bożeny|Drogomira|Zenona",
		"Dagny|Sławomiry|Wiktora",
		"Adama|Ewy|Irminy",
		"Anety|Glorii|Piotra",
		"Dionizego|Kaliksta|Szczepana",
		"Fabioli|Jana|Żanety",
		"Antoniusza|Cezarego|Teofilii",
		"Dawida|Dionizy|Tomasza",
		"Eugeniusza|Katarzyny|Sabiny",
		"Mariusza|Melanii|Sylwestra"
	)
));
