# -*- coding: utf-8 -*-
# All possible paths on the keyboard from letter a to b

from Queue import Queue
import itertools
import pickle
import os.path
import re


LAYOUT = ['qwertyuiop', 'asdfghjkl', 'zxcvbnm']

def valid_coord(coord):
    x, y = coord
    
    #Out of keyboard bounds
    if x < 0 or y < 0 or y >= len(LAYOUT) or x >= len(LAYOUT[y]):
        return False
        
    return True


def get_next_options(coord):
    options = []
    x,y = coord
    
    options.append((x+1,y))
    options.append((x-1,y))
    options.append((x,y+1))
    options.append((x,y-1))
    
    if y == 0:
        options.append((x-1,y+1)) #w>a
    if y == 1:
        options.append((x+1,y-1)) #g>y
        options.append((x-1,y+1)) #g>v
        options.append((x-2,y+1)) #g>c
    if y == 2: 
        options.append((x+1,y-1)) #x>d
        options.append((x+2,y-1)) #v>h
    
    return [c for c in options if valid_coord(c)]

def coord_to_letter(coord):
    x,y = coord
    row = LAYOUT[y]
    return row[x]
    
def letter_to_coord(letter):
    for i, row in enumerate(LAYOUT):
        if letter in row: 
            index = row.index(letter)
            return (index, i)
    
    return ((-1,-1))

def determine_paths(current, goal):

    if current == goal:
        return []
        
    queue = Queue()
    
    for option in get_next_options(current):
        queue.put((option,[]))
    
    shortest_path = 10000 #No path found yet    

    results = []
    
    while not queue.empty():
        current, so_far = queue.get()
        
        if len(so_far) > shortest_path:
            break #Stop
        
        if current == goal:
            results.append(so_far)
            shortest_path = len(so_far)
        
        for option in get_next_options(current):
            queue.put((option, so_far+[current]))
            
    
    return results
    
def paths_to_swipehint(paths): #String paths
    if len(paths) == 0:
        return ''
    shint = ''
    n = len(paths[0])
    for i in xrange(n):
        for path in paths:
            letter = path[i]
            if letter not in shint:
                shint += letter
    return shint
        
                
        
def swipehint_for_letters(letter1, letter2):
    start = letter_to_coord(letter1)  
    goal = letter_to_coord(letter2)
    
    paths = determine_paths(start,goal)
    letter_paths = []
    for path in paths:
        letters = ''.join([coord_to_letter(coord) for coord in path])
        letter_paths.append(letters)
        #print len(path), letters
    
    return paths_to_swipehint(letter_paths)


def calculate_swipehints():
    print "First time swipehint setup"
    print "(paths from all letters on the keyboard to others)"
    
    letters = 'abcdefghijklmnopqrstuvwxyz'
    #letters = 'abc'
    letter_combinations = list(itertools.combinations(letters, 2))
    
    hints = {}    
    
    for i, combo in enumerate(letter_combinations):
        print i+1, '/', len(letter_combinations), combo
        a,b = combo
        hint = swipehint_for_letters(a,b)
        
        hints[a+b] = hint
        hints[b+a] = hint[::-1]
        print '\t{0} -> {1} = {2}'.format(a,b,hints[a+b])
        print '\t{0} -> {1} = {2}'.format(b,a,hints[b+a])
    
    for letter in letters: #Duplicate letters have no swipehint
        hints[letter+letter] = ''
    
    for key, value in hints.iteritems():
        letter1, letter2 = key.split('')
        
        x1,y1 = coord_to_letter(letter1)
        x2,y2 = coord_to_letter(letter2)
        
        if y1 == y2:
            print key, hints[key]       
            row_letters = LAYOUT[y1]
            hints[key] = ''.join([letter for letter in row_letters if letter not in row_letters])
            
            print key, hints[key]
    
    with open('hints.p','w') as f:
        pickle.dump(hints, f)

def load_hints():
    with open('hints.p','r') as f:
        hints = pickle.load(f)
    return hints

def load_or_calculate_swipehints():
    if not os.path.isfile('hints.p'):
        calculate_swipehints()
        
    return load_hints()
    
#Must be placed here :/
HINTS = load_or_calculate_swipehints()

def swipehint(word):
    shint = ''
    
    #Make word alphanumeric (hasn't -> hasnt)
    word = re.sub("[^a-zA-Z]","", word).lower()
    
    for i in xrange(len(word)-1):
        letter1= word[i]
        letter2= word[i+1]
        #print word, letter1, letter2
        shint += HINTS[letter1+letter2]
        
    return shint


if __name__ == '__main__':
    #print swipehint_for_letters('z','p')
    print swipehint('hello')
    
