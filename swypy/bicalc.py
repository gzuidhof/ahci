# -*- coding: utf-8 -*-
"""
Created on Sun Jan 17 18:32:06 2016

@author: Wouter
"""
from ast import literal_eval

file = open("correctedbigrams.txt","w")
file2 = open("correctedsingles.txt","w")

bigrams = []
with open('testbigrams.txt', 'r') as f:
    for line in f:
        bigrams.append(literal_eval(line.strip()))

singles=[]
with open('testwords.txt', 'r') as g:
    for line in g:
        singles.append(literal_eval(line.strip()))

base = 1.0/len(singles)
sum = 0
for each in singles:
    sum = sum + each[1]
for each in singles:
    file2.write(str((each[0],each[1]/sum))+'\n')
file2.close()
    
singles=[]
with open('correctedsingles.txt', 'r') as g:
    for line in g:
        singles.append(literal_eval(line.strip()))

for each in bigrams[0:len(bigrams)]:
    prob = 0.0
    A = 0.0
    B = 0.1
    BA = each[0]
    for x in singles:
        if x[0] == each[1]:
            A = x[1]
    for y in singles:
        if y[0] == each[2]:
            B = y[1]
    
    prob = ((BA*A)/(B))/len(singles)
    if prob == float(0):
        prob = base
    bigrams[bigrams.index(each)] = (prob,each[1],each[2])
    file.write(str((prob,each[1],each[2])) + '\n')
#    print(str(each) + 'A: ' + str(A) + ' and B: ' + str(B) + 'and BA: ' + str(BA))
#
print(bigrams[0:2])