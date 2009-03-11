#!/usr/bin/python

# converts some csv format into homebank readable csv

# "BANKOMAT 12345 KARTE66 1.01.UM 20:00 ";"01.01.2013";"-40,00";"EUR"
infile="28263611600_20090201_20090301.csv"

#        Column list:
#        date ; mode ; info ; payee ; description ; amount ; category
#
#        Values:
#        date     => format should be DD-MM-YY
#        mode     => from 0=none to 5=personal transfer
#        info     => a string
#        payee    => a payee name
#        description  => a string
#        amount   => a number with a '.' as decimal separator, ex: -24.12 or 36.75
#        category => a category name

import datetime

fh = open(infile)
for line in fh.readlines()[1:]:
    try:
        (description, date, amount, currency) = line.replace("\"","").replace(",",".").split(";")
    except:
        continue
    (day, month, year) = map(int, date.split("."))
    date = datetime.date(year, month, day).strftime("%d-%m-%Y")
    mode = ""
    info = ""
    payee = ""
    description = description.decode('iso-8859-15').encode('utf-8','strict').strip()
    amount = float(amount)
    category = ""
    output = [date, mode, info, payee, description, str(amount), category]
    print ";".join(output)


