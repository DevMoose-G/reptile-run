import json
import math

upgrades = {"nodes":[], "nodes_obtained":[]}

upgrades_wanted = 350
i = 0
tongue_length = 5
amounts = {"Tongue":(2.0, 1.2), "Health":1.0, "Damage":0.75,
           "AttackSpeed":1.15}
levels = {"Tongue":1, "Health":1, "Damage":1,
           "AttackSpeed":1}
costs = {"Tongue":25, "Health":45, "Damage":55,
           "AttackSpeed":50}
cost_increase = 0.5 # currently exponentially growing
# pre_reqs_ids are not necessary and description is also not
categories = list(amounts.keys())
while(i < upgrades_wanted):
    current_category = categories[i%len(categories)]
    current_upgrade = {"id":i+1}
    current_upgrade["category"] = current_category
    if(current_category == "Tongue"):
        current_upgrade["amount"] = amounts[current_category][0] # length
        current_upgrade["amount2"] = amounts[current_category][1] # speed
    else:
        current_upgrade["amount"] = amounts[current_category]
    current_upgrade["level"] = levels[current_category]
    
    current_upgrade["cost"] = costs[current_category]
     
    upgrades["nodes"].append(current_upgrade)

    # do all updates to amounts, levels, & costs here
    levels[current_category] += 1
    # make less exponential (hit cap increase of 1.5x the previous one after first 150)
    costs[current_category] = int(math.exp(math.log(costs[current_category]) + cost_increase))
    # still need to update amounts
    
    i+=1

with open("../Assets/Resources/upgrades_new.json", "w") as f:
    f.seek(0)
    jsonText = json.dumps(upgrades)
    f.write(jsonText)
