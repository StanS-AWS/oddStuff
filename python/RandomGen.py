import random

class RandomGen(object):
    _probabilities = [0.01, 0.3, 0.58, 0.1, 0.01]
    #_probabilities = [0.01, 0.1, 0.58, 0.01, 0.3]
    _random_nums = [-1, 0, 1, 2, 3]
    _range_map = []

    def __init__(self):
        self.populateRangeMap()

    def populateRangeMap(self):
        if self.CheckProb():
            base_val = 0 
            for p in self._probabilities:
                map_val = base_val + p
                self._range_map.append(map_val)
                base_val = map_val
            #to eliminate any rounding error possibility
            self._range_map[-1] = 1
        else:
            raise ValueError("invalid _probabilities")
                
        return


    def CheckProb(self):
        if sum(self._probabilities,-1) < 0.0000000000001:
            return True
        else:
          return False
    
    
    def GetNext(self):
        r = random.random()
        for i in range(len(self._range_map)):
            if r <= self._range_map[i]:
                return self._random_nums[i]

#test
r = RandomGen()
freq = [0,0,0,0,0]
num_run = 1000

#fill the frequency array with the actual numbers of generated occurrences
for i in range(num_run):
    v = r.GetNext()
    #print(v)
    freq[r._random_nums.index(v)] += 1

#compare the resulting sample with the given list of probabilities
for t in range(len(freq)):
    
    if abs(freq[t]/num_run - r._probabilities[t]) > 0.5*r._probabilities[t] :
        print("unexpected result at position: " + str(t))
        print("actual: " + str(freq[t]/num_run) + " vs. " + str(r._probabilities[t]))
        print("try to rerun or increase the number of test runs");

print("Finished")
