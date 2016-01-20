from ast import literal_eval

def getmax(words):
    best = ('',0)
    for each in words:
        if each[1] > best[1]:
            best = each
    return best[0]

def getmax2(bigrams):
    best = ('',0)
    for each in bigrams:
        if each[1]>best[1]:
            best = each
    return best[0]  

def uniget(poss):        
    indices = [(x[0],x[1]) for x in result1 if x[0] in poss]
    return str(getmax(indices))

def find(poss):
    for each in result1:
        if each[0] == poss:
            return each

def biget(prev,poss):       
    indices = [(x[2],x[0]) for x in result2 if x[1] == prev and x[2] in poss]
    theseguys = [each[0] for each in indices]
    for x in poss:
        if not x in theseguys:
            indices.append(find(x))
    return str(getmax2(indices))

def decide(past, poss):
    text = past.split(';')
    prev = text[len(text)-1]
    if prev in [thisone[1] for thisone in result2]:
        return biget(prev,poss)
    else:
        return uniget(poss)

result1 = []
with open('correctedsingles.txt', 'r') as f:
    for line in f:
        result1.append(literal_eval(line.strip()))  

result2 = []
with open('correctedbigrams.txt', 'r') as f:
    for line in f:
        result2.append(literal_eval(line.strip()))

#print(decide('',['aardvark', 'aback', 'abaft']))
#print(decide('the',['abalone','abbess','abbe','abbot','aardvark','above']))
#'abalone','abbess','abbe','abbot',
print(decide('',['unapproachable','aardvark']))