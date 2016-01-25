def uniget(poss,result1):        
    indices = [(x[0],x[1]) for x in result1 if x[0] in poss]
    indices = sorted(indices,key=lambda x:(-x[1],x[0]))
    result = [x[0] for x in indices]
    return result

def find(poss,result1):
    for each in result1:
        if each[0] == poss:
            return each

def biget(prev,poss,result1,result2):       
    indices = [(x[2],x[0]) for x in result2 if x[1] == prev and x[2] in poss]
    theseguys = [each[0] for each in indices]
    for x in poss:
        if not x in theseguys:
            indices.append(find(x,result1))
    indices = sorted(indices,key=lambda x:(-x[1],x[0]))
    result = [x[0] for x in indices]
    return result

def decide(prev, poss, result1, result2):
    if not prev=="":    
        if prev in [thisone[1] for thisone in result2]:
            return biget(prev,poss,result1,result2)
    else:
        return uniget(poss)

#print(getbest('',['aardvark', 'aback', 'abaft']))
#print(getbest('the',['abalone','abbess','abbe','abbot','aardvark','above']))
#'abalone','abbess','abbe','abbot',

# Code below has been omitted but may still be useful later on:
#def getmax(words):
#    best = ('',0)
#    for each in words:
#        if each[1] > best[1]:
#            best = each
#    return best[0]
#
#def getmax2(bigrams):
#    best = ('',0)
#    for each in bigrams:
#        if each[1]>best[1]:
#            best = each
#    return best[0] 