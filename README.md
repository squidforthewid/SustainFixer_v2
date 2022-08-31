# SustainFixer_v2

## Description
A "sustain gap," in common usage, is a short gap between the end of a sustained Clone Hero/Guitar Hero/Rock Band note, and the next note immediately after it's end time.

It is almost universally agreed upon that including sustain gaps in a chart is best practice and provides the best user experience, allowing the player a window to transition from one note or chord to the next without having to prematurely terminate the sustain, even if by a miniscule amount.
 
However, some charters (including Neversoft, developers of Guitar Hero) have elected to chaotically neglect this implicit contract between the charter and the laws of nature.

This tool, in an effort to correct this... imperfection... is designed to parse through an entire directory in search of chart files, search for any instances of a sustained note ending where another begins, and shortening the length of the sustain by just enough to create the objectively ideal user experience.
 
Alternatively, this tool can be used by charters themselves who wish to quickly fix their charts, in the event that they couldn't conveniently include sustain gaps when initially charting the song.
 
A lack of a sustain gap is defined as the end of a note being within 1/128 note of the beginning of another note in the chart. Any further than that, and it's considered intentional and mindful placement by the charter.

The length of the gap created is 1/32 note for <100BPM, 1/24 note for 100-140BPM, and 1/16 note for >140BPM, as is recommended in [CustomSongsCentral's Monthly Pack Submission Guidelines.](https://customsongscentral.com/monthly-pack-submission-guidelines/)

For record-chasers, bear in mind that this WILL slightly reduce the maximum points that can be achieved on songs that initially released without sustain gaps.
 
## Usage
Simply drag and drop your song directory (or directories) onto SustainFixer_v2.exe.

The program will search for any file with a .chart or .mid extension in the directory and overwrite it accordingly.

Vocal charts are ignored.

In addition, any files that return an error will be ignored and listed at the end after all other files are processed.

## Disclaimers
* Make a backup of songs folder before using.
* Do not run any .chart or .mid files not intended to be played in Clone Hero through the program.
* Legacy notation in .chart files (i.e. 0 = E O will not be recognized as an open note) is not currently supported. Make sure your chart adheres to standards set by Moonscraper for best results.
* Please reach out to Squidicus#3153 on Discord to report bugs and errors.

## Credits
This software uses [drywetmidi](https://github.com/melanchall/drywetmidi), released under the MIT license.